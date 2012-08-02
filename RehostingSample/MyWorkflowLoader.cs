using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel.Design.Serialization;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Design;
using System.ComponentModel.Design;
using System.Xml;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Serialization;

namespace RehostingSample
{
    class MyWorkflowLoader : WorkflowDesignerLoader
    {
        string xomlfile;

        public MyWorkflowLoader(string filename)
        {
            xomlfile = filename;
        }

        public override TextReader GetFileReader(string filePath)
        {
            throw new NotImplementedException();
            //return new StreamReader(new FileStream(filePath, FileMode.Open));
        }

        public override TextWriter GetFileWriter(string filePath)
        {
            throw new NotImplementedException();
            //return new StreamWriter(new FileStream(filePath, FileMode.Create));
        }

        public override string FileName
        {
            get { return xomlfile; }
        }

        // �����́A���[�h���ɌĂ΂�܂�
        protected override void PerformLoad(IDesignerSerializationManager serializationManager)
        {
            base.PerformLoad(serializationManager);

            XmlReader reader = new XmlTextReader(this.xomlfile);
            WorkflowMarkupSerializer xomlSerializer = new WorkflowMarkupSerializer();
            Activity rootActivity = (Activity)xomlSerializer.Deserialize(reader);
            reader.Close();

            NestedActivityLoad(rootActivity);
        }

        // �l�X�g����邷�ׂẴA�N�e�B�r�e�B���f�U�C�i�[�Ƀ��[�h
        protected void NestedActivityLoad(Activity root)
        {
            IDesignerHost designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
            designerHost.Container.Add(root);
            if (!(root is CompositeActivity))
                return;

            // �ċA�Ăяo��
            foreach (Activity nested in ((CompositeActivity)root).Activities)
                NestedActivityLoad(nested);
        }

        // �����́A�A�����[�h (�ۑ�) ���ɌĂ΂�܂�
        public override void Flush()
        {
            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
            Activity rootActivity = (Activity)host.RootComponent;
            XmlTextWriter xmlWriter = new XmlTextWriter(this.xomlfile, Encoding.Default);
            WorkflowMarkupSerializer xomlSerializer = new WorkflowMarkupSerializer();
            xomlSerializer.Serialize(xmlWriter, rootActivity);
            xmlWriter.Close();
        }

    }
}
