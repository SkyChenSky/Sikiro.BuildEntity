using System;
using System.Collections.Generic;
using System.IO;
using NVelocity;
using NVelocity.App;
using NVelocity.Runtime;

namespace 陈珙.AutoBuildEntity.Common.Helper
{
    public static class NVelocityHelper
    {
        /// <summary>
        /// 初始化模板引擎
        /// </summary>
        public static string ProcessTemplate(string template, Dictionary<string, object> param)
        {
            var templateEngine = new VelocityEngine();
            templateEngine.SetProperty(RuntimeConstants.RESOURCE_LOADER, "file");

            templateEngine.SetProperty(RuntimeConstants.INPUT_ENCODING, "utf-8");
            templateEngine.SetProperty(RuntimeConstants.OUTPUT_ENCODING, "utf-8");

            templateEngine.SetProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, AppDomain.CurrentDomain.BaseDirectory);


            var context = new VelocityContext();
            foreach (var item in param)
            {
                context.Put(item.Key, item.Value);
            }

            templateEngine.Init();


            var writer = new StringWriter();
            templateEngine.Evaluate(context, writer, "mystring", template);

            return writer.GetStringBuilder().ToString();
        }

    }

}
