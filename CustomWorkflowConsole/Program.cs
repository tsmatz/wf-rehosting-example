using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace CustomWorkflowConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            WorkflowServiceHost host = new WorkflowServiceHost(typeof(CustomWorkflowLibrary.Workflow1),
                new Uri(@"http://localhost:8081/Demo/CustomWorkflowService"));
            host.Open();
            Console.WriteLine("停止するには、キーを入力 . . .");

            Console.ReadLine();
            host.Close();
        }
    }
}
