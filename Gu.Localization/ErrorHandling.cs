namespace Gu.Localization
{
    /// <summary>Specify a strategy for how translation errors are handled.</summary>
    public enum ErrorHandling
    {
        /// <summary>Inherits behaviour from <see cref="Translator.ErrorHandling"/> or defaults to throw.</summary>
        Default,

        /// <summary>Throws if something is wrong.</summary>
        Throw,

        /// <summary>Returns information about the error in the result.</summary>
        ReturnErrorInfo
    }
}