﻿using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MvcTesting.AspNetCore
{
    public class CaptureResultFilter : ActionFilterAttribute
    {
        public static ResultExecutedContext LastResult;
        public static Exception             LastException;
        public static bool                  LastExceptionHandled;

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            base.OnResultExecuted(context);

            if (context != null)
                LastResult = context;

            LastException = context.Exception;
            LastExceptionHandled = context.ExceptionHandled;
        }
    }
}
