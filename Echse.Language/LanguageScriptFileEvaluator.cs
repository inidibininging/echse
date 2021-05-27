using System;
using System.IO;
using Echse.Domain;
using States.Core.Infrastructure.Services;

namespace Echse.Language
{
    public class LanguageScriptFileEvaluator
    {
        private string FilePath { get; set; }
        public string MainFunction { get; set; }
        public Interpreter Interpreter { get; set; }
        public LanguageScriptFileEvaluator(string filePath, string startupFunction, string runOperation)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            MainFunction = startupFunction ?? throw new ArgumentNullException(nameof(startupFunction));
            Interpreter = new Interpreter(runOperation);
        }
        public LanguageScriptFileEvaluator Evaluate(IStateMachine<string, IEchseContext> context)
        {
            if (!File.Exists(FilePath))
                throw new FileNotFoundException("No mechanic specified", FilePath);
            var scriptFileContent = File.ReadAllText(FilePath);

            Interpreter.Context = context;
            Interpreter.Run(scriptFileContent);
            Interpreter.Context = null;

            return this;
        }
        public void Run(IStateMachine<string, IEchseContext> context)
        {
            try
            {
                context.GetService.Get(MainFunction).Handle(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Interpreter.Context = context;
                Interpreter.Run(MainFunction);
                Interpreter.Context = null;
            }
        } 
    }

}
