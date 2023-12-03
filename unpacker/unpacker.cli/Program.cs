using CommandLine;
using unpacker;

namespace unpacker.cli
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments(args,
                typeof(UnpackOp),
                typeof(UnpackLevelsOp)
                )
                    .MapResult<object, OpResultCode>(obj =>
                    {
                        switch (obj)
                        {
                            case UnpackOp op: return op.Run();
                            case UnpackLevelsOp op: return op.Run();
                            default: return OpResultCode.COMMAND_LINE_PARSER_FAILED;
                        }
                    }, o => OpResultCode.COMMAND_LINE_PARSER_FAILED)
                ;
        }
    }

    enum OpResultCode
    {
        SUCCESS = 0,
        COMMAND_LINE_PARSER_FAILED,
        GENERIC_ERROR,
    }
}
