namespace Gu.Localization
{
    /// <summary>Specify a strategy for how translation errors are handled.</summary>
    public enum ErrorHandling
    {
        /// <summary>Inherits behaviour from <see cref="Translator.ErrorHandling"/> or defaults to throw.</summary>
        Inherit,

        /// <summary>Throws if something is wrong.</summary>
        Throw,

        /// <summary>Returns information about the error in the result.</summary>
        ReturnErrorInfo,

        /// <summary>Returns information about the error in the result but leaves neutral strings intact.</summary>
        ReturnErrorInfoPreserveNeutral,
    }
}
