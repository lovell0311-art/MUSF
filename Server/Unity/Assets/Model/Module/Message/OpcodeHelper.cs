using System.Collections.Generic;

namespace ETModel
{
	public static class OpcodeHelper
	{
		private static readonly HashSet<ushort> ignoreDebugLogMessageSet = new HashSet<ushort>
		{
			OuterOpcode.C2R_Ping,
			OuterOpcode.R2C_Ping,
		};

		public static bool IsNeedDebugLogMessage(ushort opcode)
		{
			if (ignoreDebugLogMessageSet.Contains(opcode))
			{
				return false;
			}

			return true;
		}

		public static bool IsClientHotfixMessage(ushort opcode)
		{
			if (opcode > 10000) return true;
			switch (opcode)
			{
				case OuterOpcode.T2T_Ping:
                case OuterOpcode.C2R_Ping:
                case OuterOpcode.R2C_Ping:
					return true;
			}
			return false;
		}
	}
}