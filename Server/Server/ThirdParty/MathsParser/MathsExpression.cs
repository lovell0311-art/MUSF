using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Maths
{

    public enum ContextType
    {
        Operand,
        Operator,
        Expression,
    }

    public class Context
    {
        public double Value = 0;
        public IExpression Expr = null;
    }

    public class IExpression
    {
        static long lastId = 0;
        public long Id;
        public IExpression()
        {
            Id = ++lastId;
        }
        public virtual void Eval(Context ctx) { }
        public virtual void Calc(Context ctx,IExpression expr) { }
        public virtual double GetValue() { return 0; }
        public virtual string GetFormula() { return ""; }
    }

    /// <summary>
    /// 基础操作符
    /// </summary>
    public class Operator : IExpression
    {
        public char Sign;
        public override void Eval(Context ctx) 
        {
            ctx.Expr = this;
        }

        public override void Calc(Context ctx, IExpression expr)
        {
            double value = expr.GetValue();
            switch (Sign)
            {
                case '+':
                    ctx.Value += value;
                    break;
                case '-':
                    ctx.Value -= value;
                    break;
                case '*':
                    ctx.Value *= value;
                    break;
                case '/':
                    ctx.Value /= value;
                    break;
                case '^':
                    ctx.Value = Math.Pow(ctx.Value, value);
                    break;
                case '%':
                    ctx.Value = ctx.Value % value;
                    break;
                case '>':
                    ctx.Value = ctx.Value > value ? 1 : 0;
                    break;
                case '<':
                    ctx.Value = ctx.Value < value ? 1 : 0;
                    break;
                case '=':
                    ctx.Value = Math.Abs(ctx.Value - value) < 0.00000001 ? 1 : 0;
                    break;
                case MathParser.GeqSign:
                    ctx.Value = ctx.Value > value || Math.Abs(ctx.Value - value) < 0.00000001 ? 1 : 0;
                    break;
                case MathParser.LeqSign:
                    ctx.Value = ctx.Value < value || Math.Abs(ctx.Value - value) < 0.00000001 ? 1 : 0;
                    break;
                case MathParser.NeqSign:
                    ctx.Value = Math.Abs(ctx.Value - value) < 0.00000001 ? 0 : 1;
                    break;
            }
        }

        public override string GetFormula()
        {
            switch (Sign)
            {
                case '=':return "==";
                case MathParser.GeqSign: return "=>";
                case MathParser.LeqSign: return "=<";
                case MathParser.NeqSign: return "!=";
            }
            return Sign.ToString();
        }

    }

    /// <summary>
    /// 操作的数值
    /// </summary>
    public class Operand : IExpression
    {
        public double Value;
        public override void Eval(Context ctx)
        {
            if(ctx.Expr == null)
            {
                ctx.Value = GetValue();
            }
            else
            {
                ctx.Expr.Calc(ctx, this);
            }
        }
        public override double GetValue()
        {
            return Value;
        }
        public override string GetFormula() { return Value.ToString(); }

    }

    /// <summary>
    /// 局部变量
    /// </summary>
    public class LocalVariable : Operand
    {
        public string Name;
        public override string GetFormula() { return Name; }

    }

    /// <summary>
    /// 表达式
    /// </summary>
    public class Expression : IExpression
    {
        public List<IExpression> ExprList = new List<IExpression>();
        public Context Ctx = null;
        public override void Eval(Context ctx)
        {
            if (ctx.Expr == null)
            {
                ctx.Value = GetValue();
            }
            else
            {
                ctx.Expr.Calc(ctx, this);
            }
        }

        public override double GetValue()
        {
            if(Ctx == null)
            {
                Ctx = new Context();
            }
            else
            {
                Ctx.Value = 0;
                Ctx.Expr = null;
            }

            foreach(var expr in ExprList)
            {
                expr.Eval(Ctx);
            }
            return Ctx.Value;
        }

        public override string GetFormula()
        {
            string str = "";
            foreach (var expr in ExprList)
            {
                str += expr.GetFormula();
            }
            return str;
        }

    }

    /// <summary>
    /// 小括号
    /// </summary>
    public class Bracket : Expression
    {
        public override string GetFormula()
        {
            string str = "";
            str += "(";
            foreach (var expr in ExprList)
            {
                str += expr.GetFormula();
            }
            str += ")";
            return str;
        }
    }

}
