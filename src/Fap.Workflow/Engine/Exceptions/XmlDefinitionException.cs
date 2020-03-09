
namespace Fap.Workflow.Engine.Xpdl.Exceptions
{
    public class XmlDefinitionException : System.ApplicationException
    {
         public XmlDefinitionException(string message)
            : base(message)
        {

        }

         public XmlDefinitionException(string message, System.Exception ex)
            : base(message, ex)
        {

        }
    }
}
