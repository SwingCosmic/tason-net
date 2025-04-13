using Antlr4.Runtime;

namespace TASON;

/// <summary>
/// 在解析时抛出<see cref="RecognitionException"/>的监听器
/// </summary>
public class ThrowingErrorListener : BaseErrorListener
{
    public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        throw new FormatException(msg, e);
    }
}
