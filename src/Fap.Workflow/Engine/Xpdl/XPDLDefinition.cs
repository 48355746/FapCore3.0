using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fap.Workflow.Engine.Xpdl
{
    /// <summary>
    /// 流程定义的XML文件中，用到的常量定义
    /// </summary>
    internal class XPDLDefinition
    {
        internal static readonly string StrXmlTransitionPath = "/workflowProcess/root/transition";
        internal static readonly string StrXmlActivityPath = "/workflowProcess/root/activity";
        internal static readonly string StrXmlParticipantsPath = "participants";
        internal static readonly string StrXmlSingleParticipantPath = "participants/participant";
        internal static readonly string StrXmlDataItemPermissions = "fields";
        internal static readonly string StrXmlDataItems = "fields/field";
    }
}
