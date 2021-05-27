using System;
using System.Collections.Generic;
using System.Linq;
using Echse.Domain;
using States.Core.Infrastructure.Services;

namespace Echse.Language
{
    public class AssignInstruction : AbstractInterpreterInstruction<AssignExpression>
    {
        public AssignInstruction(Interpreter interpreter, AssignExpression assignExpression, int functionIndex) 
            : base(interpreter, assignExpression, functionIndex)
        {
        
        }

        private TagVariable GetVariable(string name, IStateMachine<string, IEchseContext> machine)
        {
            //get the variable out of the function scope 
            var variable = machine
                .SharedContext
                .Variables
                .FirstOrDefault(t => t.Name == name &&
                                     t.Scope == Scope?.Expression.Name);

            var lastFn = Scope;
            var identifierName = name;
            
            // look up the variable inside the function arguments 
            while (variable == null)
            {
                var argumentIndex = lastFn.GetFunctionArgumentIndex(identifierName);
                
                if (argumentIndex < 0)
                    throw new ArgumentNullException(nameof(identifierName),
                        $"Tag with name {identifierName} not found inside {Scope.Expression.Name}");
                
                variable = lastFn
                    .LastCaller
                    .GetVariableOfFunction(machine, argumentIndex);
                if (variable != null)
                    continue;
                identifierName = lastFn
                    .LastCaller
                    .Expression
                    .Arguments
                    .Arguments
                    .ElementAt(argumentIndex)
                    .Name;
                lastFn = lastFn.LastCaller.Scope;
                
            }
            if (variable.DataTypeSymbol != LexiconSymbol.TagDataType)
                throw new InvalidOperationException($"Syntax error. Cannot execute a modify instruction. Data type of variable is not a tag.");
            // Console.WriteLine($"Variable:{variable.Name} Current Value:{variable.Value}");
            // Console.WriteLine(System.Environment.NewLine);
            return variable;
        }
        
        public override void Handle(IStateMachine<string, IEchseContext> machine)
        {
            // get assignment
            var assignmentIndex = Owner.Instructions.IndexOf(this as IAbstractInterpreterInstruction<IAbstractLanguageExpression>);
            
            // dunno why I made this check
            if(assignmentIndex == -1)
                throw new InvalidOperationException("Assignment instruction not found. Code was modified");
            
            // find the owner of the assign instruction
            while ((Owner.Instructions[assignmentIndex] is FunctionInstruction) == false)
            {
                if (assignmentIndex < 0)
                    throw new InvalidOperationException("Function instruction cannot be found. Assignment must be in a function scope. Are you making an assignment inside a function?");
                assignmentIndex--;
            }
            var scopeOfAssignment = (Owner.Instructions[assignmentIndex] as FunctionInstruction)?.Expression.Name;
            
            // get the tag layer where the variables are stored
            var tagLayer = machine
                .SharedContext
                .Variables;

            if(tagLayer == null)
                throw new ArgumentNullException(nameof(tagLayer),"Tags cannot be saved. Please check the data layer for language stored variables");
            
            //create or replace variable
            var variable = tagLayer
                .FirstOrDefault(t => t.Name == Expression.Left.Name &&
                                     t.Scope == scopeOfAssignment // no scope for now. scope must be implemented in the language first
                );
            
            // if the variable is null, create it 
            if (variable == null)
            {
                /*
                    if the value of tag variable is a tag. cool assign value.
                    if the value of tag variable is a variable itself, trace back variable value
                    if the value of tag variable is an execute expression, trace back execution value
                 */
                var tagValueAssigned = string.Empty;

                switch (Expression.Right)
                {
                    case IdentifierExpression:
                        var tagVariable = GetVariable(Expression.Right.Name, machine);
                        if (tagVariable == null)
                            throw new ArgumentNullException(nameof(tagVariable),
                                $"Tag with name {Expression.Right.Name} not found inside {Scope.Expression.Name}");
                        tagValueAssigned = tagVariable.Value;
                        break;
                    
                    case ExecuteExpression:
                        var exe = new ExecuteInstruction(Owner, Expression.Right as ExecuteExpression, FunctionIndex);
                        exe.Handle(machine);
                        
                        var theFunction = Owner.Instructions.FirstOrDefault(i =>
                            i is FunctionInstruction functionInstruction &&
                            functionInstruction?.Expression.Name == this.Expression.Right.Name) as FunctionInstruction;
                        tagValueAssigned = theFunction?.LastReturnValue.Value.Name;
                        
                        break;

                    case TagExpression:
                    case RefExpression:
                        tagValueAssigned = Expression.Right.Name;
                        break;
                }

                if (string.IsNullOrWhiteSpace(tagValueAssigned))
                    throw new ArgumentNullException(nameof(tagValueAssigned), $"The value of Tag {Expression.Left.Name} is empty inside {Scope.Expression.Name}");
                
                machine.SharedContext.AddVariable(new TagVariable()
                {
                    Name = Expression.Left.Name,
                    Value = tagValueAssigned,
                    Scope = scopeOfAssignment,
                    DataTypeSymbol = Expression.DataType.DataType
                });
                
                variable = tagLayer.FirstOrDefault(t => t.Name == Expression.Left.Name &&
                                                                      t.Scope == scopeOfAssignment);
                if(variable == null)
                    throw new ArgumentNullException(nameof(tagLayer), $"Tag with name {Expression.Left.Name} cannot be created inside {Scope.Expression.Name}");
            }
            else
            {
                variable.Name = Expression.Left.Name;
                variable.Value = Expression.Right.Name;
            }

            
            base.Handle(machine);
        }
    }
}