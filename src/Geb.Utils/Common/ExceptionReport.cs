using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Geb.Utils
{
    public class ExceptionReport
    {
        public static String ReportUri { get; set; }

        /// <summary>
        /// 报告异常
        /// </summary>
        /// <param name="ex"></param>
        public static void Report(Exception ex)
        {
            if (String.IsNullOrEmpty(ReportUri) == true) return;
        }
    }
}
