using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Echse.Domain;
using States.Core.Infrastructure.Services;

namespace Echse.Language
{
    public class Interpreter : IStateMachine<string,Tokenizer>
    {
        public static MemoryStream GenerateStreamFromString(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
        }
        public string RunOperation { get; set; }
        public Interpreter(string runOperation)
        {
            RunOperation = runOperation;
        }
        public IStateMachine<string, IEchseContext> Context { get; set; }

        public IStateGetService<string, Tokenizer> GetService => throw new NotImplementedException();

        public IStateSetService<string, Tokenizer> SetService => throw new NotImplementedException();
        
        public IStateNewService<string, Tokenizer> NewService => throw new NotImplementedException();

        public string SharedIdentifier { get; private set;}

        public Tokenizer SharedContext { get; set; }

        public void Run(string key)
        {
            SharedIdentifier = key;
            // Console.WriteLine($"running {key}");
            using var codeStream = GenerateStreamFromString(key);
            codeStream.Seek(0, SeekOrigin.Begin);
            using var coreReader =  new StreamReader(codeStream);
            SharedContext  = new Tokenizer(coreReader);
            while(SharedContext.MoveNext() &&
                  SharedContext.Current != LexiconSymbol.NA)
            {
                // Console.WriteLine(Enum.GetName(typeof(LexiconSymbol), SharedContext.Current));
                if(SharedContext.Current == LexiconSymbol.NotFound)
                    continue;

                HandleInstruction<IfExpression, IfInstruction>((s) => s == LexiconSymbol.If);
                HandleInstruction<EndIfExpression, EndIfInstruction>((s) => s == LexiconSymbol.EndIf);
                        
                // HandleInstruction<CreateExpression, CreateInstruction<T>>((s) => s == LexiconSymbol.Create);
                _createList.ForEach(createInstruction => createInstruction());
                HandleInstruction<DestroyExpression, DestroyInstruction>((s) => s == LexiconSymbol.Destroy);
                HandleInstruction<ExecuteExpression, ExecuteInstruction>((s) => s == LexiconSymbol.Execute); //TODO: Fix this 
                HandleInstruction<FunctionExpression, FunctionInstruction>((s) => s == LexiconSymbol.FunctionIdentifier); //TODO: Fix this
                HandleInstruction<AssignExpression, AssignInstruction>((s) => s == LexiconSymbol.Set);
                HandleInstruction<ReturnExpression, ReturnInstruction>((s) => s == LexiconSymbol.Return);
                HandleInstruction<WaitExpression, WaitInstruction>((s) => s == LexiconSymbol.Wait);
                _customList.ForEach(customInstruction => customInstruction());
                _modifyList.ForEach(modInstruction => modInstruction());
                
                // HandleInstruction<ModifyAttributeExpression, ModifyInstruction<T>>((s) => s == LexiconSymbol.Modify);
            }
        }
        
        private void HandleInstruction<TExpr, TInstr>(Func<LexiconSymbol, bool> symbolPredicate)
            where TExpr : AbstractLanguageExpression, new()
            where TInstr : AbstractInterpreterInstruction<TExpr>
        {
            
            if (!symbolPredicate(SharedContext.Current))
                return;
            var expr = new TExpr();
            expr.Handle(this);
            //TODO: switch argument ordering here!!!!
            Activator.CreateInstance(typeof(TInstr), this, expr, Instructions.Count);
        }

        private void HandleCreateInstruction<TEntity>(Func<LexiconSymbol, 
                bool> symbolPredicate, 
                Func<string, bool> predicate,
                Action<string, string> functionWithArgumentsToCall)
        {
            if (!symbolPredicate(SharedContext.Current))
                return;
            var expr = new CreateExpression();
            expr.Handle(this);

            new CreateInstruction<TEntity>(this, expr, Instructions.Count, predicate, functionWithArgumentsToCall);
        }
        
        private void HandleModifyInstruction<TEntity>(Func<LexiconSymbol, 
                bool> symbolPredicate, 
            Func<string, IEnumerable<TEntity>> tagPredicate,
            Func<EntityExpression, IEnumerable<TEntity>> entityPredicate,
            Action<TEntity, ModifyAttributeExpression> entityHandler)
        {
            if (!symbolPredicate(SharedContext.Current))
                return;
            
            var expr = new ModifyAttributeExpression();
            expr.Handle(this);

            new ModifyInstruction<TEntity>(this, expr, Instructions.Count, tagPredicate, entityPredicate, entityHandler);
        }

        private List<Action> _createList = new();
        private List<Action> _modifyList = new();
        private List<Action> _customList = new();

        public Interpreter Add<TExpr, TInstr>(Func<LexiconSymbol, bool> symbolPredicate)
            where TExpr : AbstractLanguageExpression, new()
            where TInstr : AbstractInterpreterInstruction<TExpr>
        {
            _customList.Add(() => HandleInstruction<TExpr, TInstr>(symbolPredicate));
            return this;
        }
        
        public Interpreter AddCreateInstruction<TEntity>(
            Func<LexiconSymbol, bool> createSymbolPredicate,
            Func<string, bool> createPredicate,
            Action<string, string> createFunctionWithArgumentsToCall)
        {
            _createList.Add(() =>
            {
                HandleCreateInstruction<TEntity>(createSymbolPredicate, createPredicate, createFunctionWithArgumentsToCall);
            });
            return this;
        }
        
        public Interpreter AddModifyInstruction<TEntity>(
            Func<LexiconSymbol, bool> modifySymbolPredicate, 
            Func<string, IEnumerable<TEntity>> tagPredicate,
            Func<EntityExpression, IEnumerable<TEntity>> entityPredicate,
            Action<TEntity, ModifyAttributeExpression> entityHandler)
        {
            _modifyList.Add(() =>
            {
                HandleModifyInstruction(modifySymbolPredicate, tagPredicate, entityPredicate, entityHandler);
            });
            return this;
        }  

        public List<IAbstractInterpreterInstruction<IAbstractLanguageExpression>> Instructions { get; set; } = new List<IAbstractInterpreterInstruction<IAbstractLanguageExpression>>();

        public IReadOnlyDictionary<string, TimeSpan> TimeLog => throw new NotImplementedException();
    }
}
