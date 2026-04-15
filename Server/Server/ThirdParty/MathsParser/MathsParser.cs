using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maths
{
    public sealed class MathParserException : Exception
    {
        public MathParserException()
        {
        }

        public MathParserException(string message) : base(message)
        {
        }

        public MathParserException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// 数学表达式解析
    /// </summary>
    public class MathParser
    {
        /// <summary>
        /// 等于或大于
        /// </summary>
        public const char GeqSign = (char)8805;
        /// <summary>
        /// 等于或小于
        /// </summary>
        public const char LeqSign = (char)8804;
        /// <summary>
        /// 不等于
        /// </summary>
        public const char NeqSign = (char)8800;

        #region 属性
        /// <summary>
        /// 运算符优先级
        /// </summary>
        public static Dictionary<string, int> Operators;


        /// <summary>
        /// 临时变量
        /// </summary>
        private Dictionary<string, IExpression> tempVariables = new Dictionary<string, IExpression>();

        /// <summary>
        /// 局部变量
        /// </summary>
        private Dictionary<string, Operand> localVariables = new Dictionary<string, Operand>();

        public IExpression Expr = null;
        #endregion

        public MathParser()
        {
            if (Operators == null)
            {
                Operators = new Dictionary<string, int>
                {
                    ["^"] = 0,
                    ["%"] = 0,
                    [":"] = 0,
                    ["/"] = 0,
                    ["*"] = 0,
                    ["-"] = 0,
                    ["+"] = 0,
                    [">"] = 0,
                    ["<"] = 0,
                    ["" + GeqSign] = 0,
                    ["" + LeqSign] = 0,
                    ["" + NeqSign] = 0,
                    ["="] = 0,
                };
            }

        }

        /// <summary>
        /// 设置局部变量
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetLocalVar(string name,double value)
        {
            if(localVariables.ContainsKey(name))
            {
                localVariables[name].Value = value;
            }
            else
            {
                localVariables.Add(name, new LocalVariable() { Name = name, Value = value });
            }
        }

        /// <summary>
        /// 解析表达式
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public bool Parse(string expr)
        {
            Expr = MathParserLogic(Lexer(expr));
            return Expr != null;
        }

        /// <summary>
        /// 获取计算的结果
        /// </summary>
        /// <returns></returns>
        public double GetValue()
        {
            if (Expr == null) return 0;
            return Expr.GetValue();
        }

        /// <summary>
        /// 反序列化出公式
        /// </summary>
        /// <returns></returns>
        public string GetFormula()
        {
            if (Expr == null) return "";
            return Expr.GetFormula();
        }

        public void Clear()
        {
            Expr = null;
            tempVariables.Clear();
            localVariables.Clear();
        }

        #region Core
        /// <summary>
        /// 为表达式生成一个key
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        private string GenKeyByExpr(IExpression expr)
        {
            string key = $"${(expr.Id).ToString()}";
            if(!tempVariables.ContainsKey(key))
            {
                tempVariables.Add(key, expr);
            }
            return key;
        }

        private IExpression GetExpr(string key)
        {
            tempVariables.TryGetValue(key, out var expr);
            return expr;
        }

        private List<string> Lexer(string expr)
        {
            string token = "";
            List<string> tokens = new List<string>();

            expr = expr.Replace("+-", "-");
            expr = expr.Replace("-+", "-");
            expr = expr.Replace("--", "+");
            expr = expr.Replace("==", "=");
            expr = expr.Replace(">=", "" + GeqSign);
            expr = expr.Replace("<=", "" + LeqSign);
            expr = expr.Replace("!=", "" + NeqSign);

            for(int i = 0; i < expr.Length; ++i)
            {
                var c = expr[i];
                if(char.IsWhiteSpace(c))
                {
                    // 去除空格
                    continue;
                }

                if(char.IsLetter(c))
                {
                    // 字母
                    if(i != 0 && (char.IsDigit(expr[i-1]) || expr[i-1] == ')'))
                    {
                        // 上一个字符是 数字 或 括号
                        // TODO 补个乘号
                        tokens.Add("*");
                    }

                    token += c;
                    while(i + 1 < expr.Length && char.IsLetterOrDigit(expr[i+1]))
                    {
                        // TODO 取出后面相连的字母和数字
                        token += expr[++i];
                    }

                    tokens.Add(token);
                    token = "";

                    continue;
                }

                if(char.IsDigit(c))
                {
                    // 数字
                    token += c;
                    while(i+1 < expr.Length && (char.IsDigit(expr[i+1])|| expr[i+1] == '.'))
                    {
                        // TODO 取出数字
                        token += expr[++i];
                    }

                    tokens.Add(token);
                    token = "";

                    continue;
                }

                if(c == '.')
                {
                    // 浮点数
                    token += c;

                    while(i+1 < expr.Length && char.IsDigit(expr[i+1]))
                    {
                        token += expr[++i];
                    }

                    tokens.Add(token);
                    token = "";

                    continue;
                }

                if(i+1 < expr.Length &&
                    (c == '-' ||c == '+')&&
                    char.IsDigit(expr[i+1]))
                {
                    // 检查 -1 +2 这种情况
                    if(i == 0 || (tokens.Count>0 && Operators.ContainsKey(tokens.Last())) || (i-1>0&& expr[i-1] == '('))
                    {
                        token += c;
                        while(i+1 < expr.Length && (char.IsDigit(expr[i+1]) || expr[i+1] == '.'))
                        {
                            token += expr[++i];
                        }

                        tokens.Add(token);
                        token = "";

                        continue;
                    }
                }

                if(c == '(')
                {
                    if(i != 0 && (char.IsDigit(expr[i - 1]) || expr[i - 1] == ')'))
                    {
                        tokens.Add("*");
                    }
                    tokens.Add("(");
                }
                else
                {
                    tokens.Add(c.ToString());
                }
            }
            return tokens;
        }

        private IExpression MathParserLogic(List<string> tokens)
        {
            tempVariables.Clear();

            while(tokens.IndexOf("(") != -1)
            {
                int open = tokens.LastIndexOf("(");
                int close = tokens.IndexOf(")",open);

                if(open >= close)
                {
                    throw new MathParserException("没有闭合括号");
                }

                var tmpExpr = new List<string>();
                for(var i = open + 1;i < close;++i)
                {
                    // TODO 将括号中的公式提取出来
                    tmpExpr.Add(tokens[i]);
                }
                // TODO 检查是不是函数
                // ...以后在加

                var expr = BasicExpression(tmpExpr);

                tokens[open] = GenKeyByExpr(expr);
                tokens.RemoveRange(open + 1, close - open);
            }
            return BasicExpression(tokens, true);
        }

        private IExpression BasicExpression(List<string> tokens,bool root = false)
        {
            switch (tokens.Count)
            {
                case 0:
                    return new Operand() { Value = 0 };
                case 1:
                    return GetOrCreateExpr(tokens[0]);
            }

            Expression expr = null;
            var bracket = new Bracket();

            foreach(var op in Operators)
            {
                int opPos;
                while((opPos = tokens.IndexOf(op.Key)) != -1)
                {
                    if (root == true || tokens.Count > 3)
                    {
                        expr = new Expression();
                    }
                    else
                    {
                        expr = bracket;
                    }

                    if (opPos == 0)
                    {
                        if(op.Key != "-")
                        {
                            throw new MathParserException($"无法解析，运算符出现在不该出现的位置 '{op.Key}'");
                        }
                        
                        expr.ExprList.Add(new Operator() { Sign = '-' });
                        expr.ExprList.Add(GetOrCreateExpr(tokens[opPos + 1]));
                        tokens[0] = GenKeyByExpr(expr);
                        tokens.RemoveRange(opPos + 1, 1);
                    }
                    else
                    {
                        expr.ExprList.Add(GetOrCreateExpr(tokens[opPos - 1]));
                        expr.ExprList.Add(new Operator() { Sign = op.Key[0] });
                        expr.ExprList.Add(GetOrCreateExpr(tokens[opPos + 1]));
                        tokens[opPos - 1] = GenKeyByExpr(expr);
                        tokens.RemoveRange(opPos, 2);
                    }
                }
            }
            if(tokens.Count != 1)
            {
                throw new MathParserException("无法解析，错误的表达式");
            }
            return expr;
        }

        private IExpression GetOrCreateExpr(string token)
        {
            // 临时变量
            var expr = GetExpr(token);
            if (expr != null) return expr;
            // 局部变量
            localVariables.TryGetValue(token, out var optExpr);
            if (optExpr != null) return optExpr;
            // 上面的都不是，看看是不是数值
            double value;
            if (!double.TryParse(token, out value))
            {
                throw new MathParserException($"局部变量 '{token}' 没有定义");
            }
            expr = new Operand() { Value = value };
            return expr;
        }
        #endregion

    }
}
