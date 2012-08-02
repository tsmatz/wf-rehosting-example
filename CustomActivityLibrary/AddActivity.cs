using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;
using System.Drawing.Drawing2D;

namespace CustomActivityLibrary
{
    [Designer(typeof(AddActivityDesigner))]
    public partial class Add3Activity : Activity
    {
        [ValidationOption(ValidationOption.Required)]
        public int CalcValueProperty
        {
            get { return (int)GetValue(CalcValuePropertyProperty); }
            set { SetValue(CalcValuePropertyProperty, value); }
        }

        public static readonly DependencyProperty CalcValuePropertyProperty =
            DependencyProperty.Register("CalcValueProperty", typeof(int), typeof(Add3Activity));

        public Add3Activity()
        {
            InitializeComponent();
        }

        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            CalcValueProperty += 3;
            return base.Execute(executionContext);
        }

    }

    [ActivityDesignerTheme(typeof(AddActivityDesignerTheme))]
    internal sealed class AddActivityDesigner : ActivityDesigner
    {
        protected override void Initialize(System.Workflow.ComponentModel.Activity activity)
        {
            base.Initialize(activity);
            this.Text = "3 を足し算";
        }
    }

    internal sealed class AddActivityDesignerTheme : ActivityDesignerTheme
    {
        public AddActivityDesignerTheme(WorkflowTheme theme)
            : base(theme)
        {
            this.BackgroundStyle = LinearGradientMode.ForwardDiagonal;
            this.BackColorStart = Color.SeaGreen;
            this.BackColorEnd = Color.BurlyWood;
            this.ForeColor = Color.Black;
        }
    }

}
