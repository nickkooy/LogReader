using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogReader.Expressions
{
    public class BoolExpr
    {
        public enum BOP { LEAF, AND, OR, NOT };

        //
        //  inner state
        //

        private BOP _op;
        private BoolExpr _left;
        private BoolExpr _right;
        private String _lit;

        //
        //  private constructor
        //

        private BoolExpr(BOP op, BoolExpr left, BoolExpr right)
        {
            _op = op;
            _left = left;
            _right = right;
            _lit = null;
        }

        private BoolExpr(String literal)
        {
            _op = BOP.LEAF;
            _left = null;
            _right = null;
            _lit = literal;
        }

        //
        //  accessor
        //

        public BOP Op
        {
            get { return _op; }
            set { _op = value; }
        }

        public BoolExpr Left
        {
            get { return _left; }
            set { _left = value; }
        }

        public BoolExpr Right
        {
            get { return _right; }
            set { _right = value; }
        }

        public String Lit
        {
            get { return _lit; }
            set { _lit = value; }
        }

        //
        //  public factory
        //

        public static BoolExpr CreateAnd(BoolExpr left, BoolExpr right)
        {
            return new BoolExpr(BOP.AND, left, right);
        }

        public static BoolExpr CreateNot(BoolExpr child)
        {
            return new BoolExpr(BOP.NOT, child, null);
        }

        public static BoolExpr CreateOr(BoolExpr left, BoolExpr right)
        {
            return new BoolExpr(BOP.OR, left, right);
        }

        public static BoolExpr CreateBoolVar(String str)
        {
            return new BoolExpr(str);
        }

        public BoolExpr(BoolExpr other)
        {
            // No share any object on purpose
            _op = other._op;
            _left = other._left == null ? null : new BoolExpr(other._left);
            _right = other._right == null ? null : new BoolExpr(other._right);
            _lit = new StringBuilder(other._lit).ToString();
        }

        //
        //  state checker
        //

        public bool IsLeaf()
        {
            return (_op == BOP.LEAF);
        }

        public bool IsAtomic()
        {
            return (IsLeaf() || (_op == BOP.NOT && _left.IsLeaf()));
        }

        public static bool Eval(BoolExpr expr, Dictionary<string, bool> booleanValues)
        {
            if (expr.IsLeaf())
            {
                return booleanValues[expr.Lit];
            }

            if (expr.Op == BoolExpr.BOP.NOT)
            {
                return !Eval(expr.Left, booleanValues);
            }

            if (expr.Op == BoolExpr.BOP.OR)
            {
                return Eval(expr.Left, booleanValues) || Eval(expr.Right, booleanValues);
            }

            if (expr.Op == BoolExpr.BOP.AND)
            {
                return Eval(expr.Left, booleanValues) && Eval(expr.Right, booleanValues);
            }

            throw new ArgumentException();
        }
    }
}
