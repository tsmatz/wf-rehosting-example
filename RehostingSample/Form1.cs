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

            // ���[�N�t���[�v���W�F�N�g�̃p�X���擾
            projectpath = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\CustomWorkflowLibrary");

            // ���[�N�t���[�f�U�C�i�[�̏�����
            this.designSurface = new DesignSurface();
            loader = new MyWorkflowLoader(Path.Combine(projectpath, "Workflow1.xoml"));
            designSurface.BeginLoad(loader);

            this.workflowView = new WorkflowView((IServiceProvider) this.designSurface);
            splitContainer1.Panel1.Controls.Add(this.workflowView);
            this.workflowView.Dock = DockStyle.Fill;

            IDesignerHost designerHost = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
            designerHost.Activate();

            // �R���e�L�X�g���j���[�̕\��
            IMenuCommandService menuService = new WorkflowMenuCommandService((IServiceProvider)workflowView);
            designerHost.AddService(typeof(IMenuCommandService), menuService);

            // �Q�Ƃ���A�Z���u�� (�ꕔ) ��ݒ�
            // (�p�����[�^�� Validation �`�F�b�N�Ȃǂ̍ۂɁA���̃A�Z���u�������ɂ�������)
            TypeProvider typeProvider = new TypeProvider((IServiceProvider)workflowView);
            typeProvider.AddAssemblyReference(@"..\..\..\CustomWorkflowLibrary\bin\Debug\CustomWorkflowLibrary.dll");
            designerHost.AddService(typeof(ITypeProvider), typeProvider);

            // (.NET 3.5 ReceiveActivity �� ServiceOperationInfo �𐳂����ݒ肵�Ȃ� (Validation Error �ƂȂ�) ���߈ȉ���ǉ�)
            SequentialWorkflowActivity rootActivity = (SequentialWorkflowActivity)designerHost.RootComponent;
            ReceiveActivity receiveActivity = (ReceiveActivity)rootActivity.Activities[0];
            TypedOperationInfo typedOperationInfo = new TypedOperationInfo();
            typedOperationInfo.ContractType = typeof(CustomWorkflowLibrary.IWorkflow1);
            typedOperationInfo.Name = "CalcData";
            receiveActivity.ServiceOperationInfo = typedOperationInfo;
        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            // �A�N�e�B�r�e�B�̒ǉ�
            object o = Activator.CreateInstance(Type.GetType("CustomActivityLibrary.Add3Activity, CustomActivityLibrary"));
            IServiceProvider provider = (IServiceProvider) workflowView;
			IDesignerHost designerHost = (IDesignerHost) provider.GetService(typeof(IDesignerHost));
            SequentialWorkflowActivity rootActivity = (SequentialWorkflowActivity)designerHost.RootComponent;
            ((CompositeActivity) rootActivity.Activities[0]).Activities.Add((Activity) o);
            designerHost.RootComponent.Site.Container.Add((IComponent) o);

            // �v���p�e�B�̐ݒ�
            Activity activity = (Activity)o;
            PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(activity)["CalcValueProperty"];
            ActivityBind bind = new ActivityBind();
            bind.Name = "Workflow1";
            bind.Path = "paramValue";
            propertyDescriptor.SetValue(activity, bind);
        }

        private void multiBtn_Click(object sender, EventArgs e)
        {
            // �A�N�e�B�r�e�B�̒ǉ�
            object o = Activator.CreateInstance(Type.GetType("CustomActivityLibrary.Multiple3Activity, CustomActivityLibrary"));
            IServiceProvider provider = (IServiceProvider)workflowView;
            IDesignerHost designerHost = (IDesignerHost)provider.GetService(typeof(IDesignerHost));
            SequentialWorkflowActivity rootActivity = (SequentialWorkflowActivity)designerHost.RootComponent;
            ((CompositeActivity)rootActivity.Activities[0]).Activities.Add((Activity)o);
            designerHost.RootComponent.Site.Container.Add((IComponent)o);

            // �v���p�e�B�̐ݒ�
            Activity activity = (Activity)o;
            PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(activity)["CalcValueProperty"];
            ActivityBind bind = new ActivityBind();
            bind.Name = "Workflow1";
            bind.Path = "paramValue";
            propertyDescriptor.SetValue(activity, bind);
        }

        private void completeBtn_Click(object sender, EventArgs e)
        {
            // �ۑ�
            loader.Flush();

            // �R���p�C�����s
            WorkflowCompiler compiler = new WorkflowCompiler();
            WorkflowCompilerParameters parameters = new WorkflowCompilerParameters();

            string[] compileFiles = new string[3];
            compileFiles[0] = loader.FileName;
            compileFiles[1] = Path.Combine(projectpath, "Workflow1.xoml.cs");
            compileFiles[2] = Path.Combine(projectpath, "IWorkflow1.cs");

            // (�ȉ��A���[���t�@�C��������ꍇ�̃T���v��)
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
                    MessageBox.Show(results.Errors[i].ErrorText, "�R���p�C���̕�", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
            }
            if(i == results.Errors.Count)
                MessageBox.Show("�R���p�C�������ł�!");
            else
                MessageBox.Show("�G���[���������܂���");
        }

    }

    // �J�X�^���̃R���e�L�X�g���j���[�̃N���X
    internal sealed class WorkflowMenuCommandService : MenuCommandService
    {
        public WorkflowMenuCommandService(IServiceProvider serviceProvider)
            : base(serviceProvider) { }

        public override void ShowContextMenu(CommandID menuID, int x, int y)
        {
            // �A�C�e���̃V���[�g�J�b�g���I�����ꂽ�ꍇ
            if (menuID == WorkflowMenuCommands.SelectionMenu)
            {
                // (�����I���̏ꍇ�����邪�A�ŏ������݂Ȃ� . . .)
                ISelectionService selectionService = (ISelectionService)GetService(typeof(ISelectionService));
                object[] selection = new object[selectionService.SelectionCount];
                selectionService.GetSelectedComponents().CopyTo(selection, 0);
                if ((selection[0] is CustomActivityLibrary.Add3Activity) || (selection[0] is CustomActivityLibrary.Multiple3Activity))
                {
                    MenuItem menuItem;
                    ContextMenu contextMenu = new ContextMenu();

                    menuItem = new MenuItem("���O�̕ύX", new EventHandler(OnMenuClicked));
                    menuItem.Tag = selection[0];
                    contextMenu.MenuItems.Add(menuItem);

                    menuItem = new MenuItem("�폜", new EventHandler(OnMenuClicked));
                    menuItem.Tag = selection[0];
                    contextMenu.MenuItems.Add(menuItem);

                    WorkflowView workflowView = (WorkflowView)GetService(typeof(WorkflowView));
                    contextMenu.Show(workflowView, workflowView.PointToClient(new Point(x, y)));
                }
            }

            // ����ȗ����܂����AValidation Error �̏ꍇ�̃R���e�L�X�g���j���[�̏������A
            // �����ɋL�ڂ��܂� (menuID �� WorkflowMenuCommands.DesignerActionsMenu �����܂��I)
            // . . .
        }

        private void OnMenuClicked(object sender, EventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;

            if (menuItem.Text == "���O�̕ύX")
            {
                Activity act = (Activity) menuItem.Tag;
                PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(act)["Name"];
                propertyDescriptor.SetValue(act, "�e�X�g");
            }
            else if (menuItem.Text == "�폜")
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
