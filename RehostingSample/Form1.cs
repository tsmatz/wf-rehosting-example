using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Activities;
using System.Drawing.Design;
using System.Workflow.ComponentModel;
using System.IO;
using System.Workflow.ComponentModel.Compiler;

namespace RehostingSample
{
    public partial class Form1 : Form
    {
        private DesignSurface designSurface;
        private WorkflowView workflowView;
        private MyWorkflowLoader loader;
        string projectpath;

        public Form1()
        {
            InitializeComponent();

            // ワークフロープロジェクトのパスを取得
            projectpath = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\CustomWorkflowLibrary");

            // ワークフローデザイナーの初期化
            this.designSurface = new DesignSurface();
            loader = new MyWorkflowLoader(Path.Combine(projectpath, "Workflow1.xoml"));
            designSurface.BeginLoad(loader);

            this.workflowView = new WorkflowView((IServiceProvider) this.designSurface);
            splitContainer1.Panel1.Controls.Add(this.workflowView);
            this.workflowView.Dock = DockStyle.Fill;

            IDesignerHost designerHost = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
            designerHost.Activate();

            // コンテキストメニューの表示
            IMenuCommandService menuService = new WorkflowMenuCommandService((IServiceProvider)workflowView);
            designerHost.AddService(typeof(IMenuCommandService), menuService);

            // 参照するアセンブリ (一部) を設定
            // (パラメータの Validation チェックなどの際に、このアセンブリを見にいくため)
            TypeProvider typeProvider = new TypeProvider((IServiceProvider)workflowView);
            typeProvider.AddAssemblyReference(@"..\..\..\CustomWorkflowLibrary\bin\Debug\CustomWorkflowLibrary.dll");
            designerHost.AddService(typeof(ITypeProvider), typeProvider);

            // (.NET 3.5 ReceiveActivity の ServiceOperationInfo を正しく設定しない (Validation Error となる) ため以下を追加)
            SequentialWorkflowActivity rootActivity = (SequentialWorkflowActivity)designerHost.RootComponent;
            ReceiveActivity receiveActivity = (ReceiveActivity)rootActivity.Activities[0];
            TypedOperationInfo typedOperationInfo = new TypedOperationInfo();
            typedOperationInfo.ContractType = typeof(CustomWorkflowLibrary.IWorkflow1);
            typedOperationInfo.Name = "CalcData";
            receiveActivity.ServiceOperationInfo = typedOperationInfo;
        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            // アクティビティの追加
            object o = Activator.CreateInstance(Type.GetType("CustomActivityLibrary.Add3Activity, CustomActivityLibrary"));
            IServiceProvider provider = (IServiceProvider) workflowView;
			IDesignerHost designerHost = (IDesignerHost) provider.GetService(typeof(IDesignerHost));
            SequentialWorkflowActivity rootActivity = (SequentialWorkflowActivity)designerHost.RootComponent;
            ((CompositeActivity) rootActivity.Activities[0]).Activities.Add((Activity) o);
            designerHost.RootComponent.Site.Container.Add((IComponent) o);

            // プロパティの設定
            Activity activity = (Activity)o;
            PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(activity)["CalcValueProperty"];
            ActivityBind bind = new ActivityBind();
            bind.Name = "Workflow1";
            bind.Path = "paramValue";
            propertyDescriptor.SetValue(activity, bind);
        }

        private void multiBtn_Click(object sender, EventArgs e)
        {
            // アクティビティの追加
            object o = Activator.CreateInstance(Type.GetType("CustomActivityLibrary.Multiple3Activity, CustomActivityLibrary"));
            IServiceProvider provider = (IServiceProvider)workflowView;
            IDesignerHost designerHost = (IDesignerHost)provider.GetService(typeof(IDesignerHost));
            SequentialWorkflowActivity rootActivity = (SequentialWorkflowActivity)designerHost.RootComponent;
            ((CompositeActivity)rootActivity.Activities[0]).Activities.Add((Activity)o);
            designerHost.RootComponent.Site.Container.Add((IComponent)o);

            // プロパティの設定
            Activity activity = (Activity)o;
            PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(activity)["CalcValueProperty"];
            ActivityBind bind = new ActivityBind();
            bind.Name = "Workflow1";
            bind.Path = "paramValue";
            propertyDescriptor.SetValue(activity, bind);
        }

        private void completeBtn_Click(object sender, EventArgs e)
        {
            // 保存
            loader.Flush();

            // コンパイル実行
            WorkflowCompiler compiler = new WorkflowCompiler();
            WorkflowCompilerParameters parameters = new WorkflowCompilerParameters();

            string[] compileFiles = new string[3];
            compileFiles[0] = loader.FileName;
            compileFiles[1] = Path.Combine(projectpath, "Workflow1.xoml.cs");
            compileFiles[2] = Path.Combine(projectpath, "IWorkflow1.cs");

            // (以下、ルールファイルがある場合のサンプル)
            //string ruleFile = ... ;
            //string resources = @"/resource:" + ruleFile + ",namespace.type.rules";
            //parameters.CompilerOptions += resources;

            parameters.ReferencedAssemblies.Add(Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles"), @"Reference Assemblies\Microsoft\Framework\v3.0\System.ServiceModel.dll"));
            parameters.ReferencedAssemblies.Add(Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles"), @"Reference Assemblies\Microsoft\Framework\v3.5\System.WorkflowServices.dll"));
            parameters.ReferencedAssemblies.Add(@"..\..\..\CustomActivityLibrary\bin\Debug\CustomActivityLibrary.dll");

            parameters.OutputAssembly = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\CustomWorkflowConsole\bin\Debug\CustomWorkflowLibrary.dll");

            WorkflowCompilerResults results = compiler.Compile(parameters, compileFiles);

            int i;
            for (i = 0; i < results.Errors.Count; i++)
            {
                if (!results.Errors[i].IsWarning)
                {
                    MessageBox.Show(results.Errors[i].ErrorText, "コンパイルの報告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
            }
            if(i == results.Errors.Count)
                MessageBox.Show("コンパイル完了です!");
            else
                MessageBox.Show("エラーが発生しました");
        }

    }

    // カスタムのコンテキストメニューのクラス
    internal sealed class WorkflowMenuCommandService : MenuCommandService
    {
        public WorkflowMenuCommandService(IServiceProvider serviceProvider)
            : base(serviceProvider) { }

        public override void ShowContextMenu(CommandID menuID, int x, int y)
        {
            // アイテムのショートカットが選択された場合
            if (menuID == WorkflowMenuCommands.SelectionMenu)
            {
                // (複数選択の場合もあるが、最初しかみない . . .)
                ISelectionService selectionService = (ISelectionService)GetService(typeof(ISelectionService));
                object[] selection = new object[selectionService.SelectionCount];
                selectionService.GetSelectedComponents().CopyTo(selection, 0);
                if ((selection[0] is CustomActivityLibrary.Add3Activity) || (selection[0] is CustomActivityLibrary.Multiple3Activity))
                {
                    MenuItem menuItem;
                    ContextMenu contextMenu = new ContextMenu();

                    menuItem = new MenuItem("名前の変更", new EventHandler(OnMenuClicked));
                    menuItem.Tag = selection[0];
                    contextMenu.MenuItems.Add(menuItem);

                    menuItem = new MenuItem("削除", new EventHandler(OnMenuClicked));
                    menuItem.Tag = selection[0];
                    contextMenu.MenuItems.Add(menuItem);

                    WorkflowView workflowView = (WorkflowView)GetService(typeof(WorkflowView));
                    contextMenu.Show(workflowView, workflowView.PointToClient(new Point(x, y)));
                }
            }

            // 今回省略しますが、Validation Error の場合のコンテキストメニューの処理も、
            // ここに記載します (menuID は WorkflowMenuCommands.DesignerActionsMenu が来ます！)
            // . . .
        }

        private void OnMenuClicked(object sender, EventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;

            if (menuItem.Text == "名前の変更")
            {
                Activity act = (Activity) menuItem.Tag;
                PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(act)["Name"];
                propertyDescriptor.SetValue(act, "テスト");
            }
            else if (menuItem.Text == "削除")
            {
                object o = menuItem.Tag;
                WorkflowView workflowView = (WorkflowView)GetService(typeof(WorkflowView));
                IServiceProvider provider = (IServiceProvider)workflowView;
                IDesignerHost designerHost = (IDesignerHost)provider.GetService(typeof(IDesignerHost));
                CompositeActivity parentActivity = (CompositeActivity)((Activity)o).Parent;
                parentActivity.Activities.Remove((Activity)o);
                designerHost.RootComponent.Site.Container.Remove((IComponent)o);
            }
        }
    }

}
