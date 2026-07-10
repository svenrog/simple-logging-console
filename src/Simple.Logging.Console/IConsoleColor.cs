namespace Simple.Logging.Console;

// A color knows how to emit its own SGR escape codes. Formatters are generic over the concrete
// color struct (where TColor : struct, IConsoleColor), so these calls are constrained calls: the
// JIT specializes per struct with no boxing and no interface dispatch on the hot path.
public interface IConsoleColor
{
    void WriteForeground(TextWriter writer);

    void WriteBackground(TextWriter writer);
}
