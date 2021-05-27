using System;
using System.Linq;
using Echse.Domain;
using States.Core.Common;
using States.Core.Infrastructure.Services;

namespace Echse.Language
{
    public class WaitInstruction : AbstractInterpreterInstruction<WaitExpression>
    {
        private WaitExpression Wait { get; }
        private TimeSpan WaitTimeSpan { get; set; } = TimeSpan.Zero;
        public bool TimeReached { get; private set;}
        private string Id { get; set; } = $"{nameof(WaitInstruction)}_{Guid.NewGuid().ToString()}";
        public WaitInstruction(Interpreter interpreter, WaitExpression wait, int functionIndex) : base(interpreter, wait, functionIndex)
        {
            Wait = wait;
        }

        public override void Handle(IStateMachine<string, IEchseContext> machine)
        {
            //Console.WriteLine(machine.SharedIdentifier);
            if(WaitTimeSpan == TimeSpan.Zero){
                Console.WriteLine("secs " + machine.SharedContext.LanguageTick.TotalSeconds);
                if (Wait.Unit.Name == "Miliseconds")
                    WaitTimeSpan = TimeSpan.FromMilliseconds((machine.SharedContext.LanguageTick.TotalMilliseconds + Wait.Number.NumberValue.Value));
                if (Wait.Unit.Name == "Seconds")
                    WaitTimeSpan = TimeSpan.FromSeconds((machine.SharedContext.LanguageTick.TotalSeconds + Wait.Number.NumberValue.Value));
                if(Wait.Unit.Name == "Minutes")
                    WaitTimeSpan = TimeSpan.FromMinutes((machine.SharedContext.LanguageTick.TotalMinutes + Wait.Number.NumberValue.Value));
                if(Wait.Unit.Name == "Hours")
                    WaitTimeSpan = TimeSpan.FromHours((machine.SharedContext.LanguageTick.TotalHours + Wait.Number.NumberValue.Value));
                Console.WriteLine("Wait timespan added");
            }

            var runOperation = machine.GetService.Get(Owner.RunOperation) as RoutineState<string,IEchseContext>;
            if(WaitTimeSpan < machine.SharedContext.LanguageTick)
            {
                if (runOperation == null)
                {
                    Console.WriteLine($"Run operation {Owner.RunOperation} not found. Cannot continue with execution");
                }
                else
                {
                    if (runOperation.Operations.Contains(Id))
                    {
                        runOperation.Operations = runOperation.Operations.Where(op => op != Id).ToList();
                        Console.WriteLine($"{Id} deleted");
                        WaitTimeSpan = TimeSpan.Zero;
                    }
                }
                //Console.WriteLine("Wait timespan reached");
                //make a loop hole to an internal stack that is looped everytime the engine calls the cli passthrough
                //if the this wait timespan is reached. a "pointer" will be made 
                //Console.WriteLine("function index = "+FunctionIndex);
                var currentInstructionIndex = FunctionIndex+1;

                while(currentInstructionIndex < Owner.Instructions.Count){
                    var currentInstruction = Owner.Instructions[currentInstructionIndex];
                    currentInstructionIndex++;
                    if(currentInstruction is FunctionInstruction)
                    {
                        //Console.WriteLine("function instruction found. aborting wait instruction");
                        break;
                    }                    
                    currentInstruction.Handle(machine);
                    if(currentInstruction is WaitInstruction)
                    {
                        //Console.WriteLine("wait instruction found. aborting wait instruction");
                        break;
                    }
                    if(currentInstruction is ReturnInstruction){
                        break;
                    }
                }
                //Console.WriteLine("operations after wait were executed");
                //TODO: this is the behaviour needed for nested functions or IF ELSE Statements right???


            }
            else
            {
                if(runOperation == null){
                    Console.WriteLine("Mainloop not found. Cannot continue with execution");
                }
                //loop again till it reaches the max
                //machine.GetService.Get(machine.SharedIdentifier).Handle(machine);
                if(!runOperation.Operations.Contains(Id)){
                    runOperation.Operations = runOperation.Operations.Append(Id).ToList();

                    try
                    {
                        if (machine.GetService.HasState(Id))
                            return;
                        else
                            machine.NewService.New(Id, this);
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine("OH OH... A TODO! Wait pointer exists");
                        Console.WriteLine(ex.Message);
                    }

                    Console.WriteLine($"{Id} inserted");
                }
            }
        }
    }
}
