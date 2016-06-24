namespace Gu.Localization
{
   internal static class ErrorHandlingExt
    {
       internal static ErrorHandling Coerce(this ErrorHandling errorHandling)
       {
            if (errorHandling != ErrorHandling.Inherit)
            {
                return errorHandling;
            }

           return Translator.ErrorHandling == ErrorHandling.Inherit
                      ? ErrorHandling.ReturnErrorInfoPreserveNeutral
                      : Translator.ErrorHandling;
       }
    }
}
