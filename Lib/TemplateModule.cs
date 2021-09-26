
using KeepCoding;

namespace KModkitLib
{

    public class TemplateModule : KtaneModule
    {
        protected override void Start()
        {
            base.Start();
            Log("Hello world!");
        }
    }
}