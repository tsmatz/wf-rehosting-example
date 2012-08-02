using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace CustomWorkflowLibrary
{
	// メモ: ここでインターフェイス名 "IWorkflow1" を変更する場合は、App.config で "IWorkflow1" への参照も更新する必要があります。
	[ServiceContract]
	public interface IWorkflow1
	{
        [OperationContract]
        int CalcData(int value);
	}
}
