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
    [Designer(typeof(MultipleActivityDesigner))]
    public partial class Multiple3Activity : Activity
	{
        [ValidationOption(ValidationOption.Required)]
        public int CalcValueProperty
        {
            get { return (int)GetValue(CalcValuePropertyProperty); }
            set { SetValue(CalcValuePropertyProperty, value); }
        }

        public static readonly DependencyProperty CalcValuePropertyProperty =
            DependencyProperty.Register("CalcValueProperty", typeof(int), typeof(Multiple3Activity));

        public Multiple3Activity()
		{
			InitializeComponent();
		}

        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            CalcValueProperty *= 3;
            return base.Execute(executionContext);
        }
	}

    [ActivityDesignerTheme(typeof(MultipleActivityDesignerTheme))]
    internal sealed class MultipleActivityDesigner : ActivityDesigner
    {
        protected override void Initialize(System.Workflow.ComponentModel.Activity activity)
        {
            base.Initialize(activity);
            this.Text = "3 をかけ算";
        }
    }

    internal sealed class MultipleActivityDesignerTheme : ActivityDesignerTheme
    {
        public MultipleActivityDesignerTheme(WorkflowTheme theme)
            : base(theme)
        {
            this.BackgroundStyle = LinearGradientMode.ForwardDiagonal;
            this.BackColorStart = Color.DarkRed;
            this.BackColorEnd = Color.Black;
            this.ForeColor = Color.White;
        }
    }
}
