﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuLib.Strategy.Op
{
    public interface OpType { }
    public interface DigitOpType : OpType { }
    public interface EliminateOp : DigitOpType { }
    public interface DigitSelectOp : DigitOpType { }
    public interface DigitUnselectOp : DigitOpType { }
    public interface UnEliminateOp : DigitOpType { }
    public interface FillOp : DigitOpType { }

    public interface SubgridOpType : OpType { }
    public interface SubgridSelectOp : SubgridOpType { }
    public interface SubgridUnselectOp : SubgridOpType { }
    public interface SubgridHLErrorOp : SubgridOpType { }
    public interface SubgridHLDefaultOp : SubgridOpType { }

    public abstract record OpBase { virtual public bool empty { get; } }
    public record EmptyOp : OpBase { override public bool empty { get { return true; } } }
    public record DigitOpBase(int Row, int Column, int Digit) : OpBase { override public bool empty { get { return false; } } }
    public record DigitOp<T>(int Row, int Column, int Digit) : DigitOpBase(Row, Column, Digit) where T : DigitOpType
    {
        public DigitOp() : this(0, 0, 0) { } // for instantiating a generic type
    }
    public record SubgridOpBase(int Row, int Column) : OpBase { override public bool empty { get { return false; } } }
    public record SubgridOp<T>(int Row, int Column) : SubgridOpBase(Row, Column) where T : SubgridOpType
    {
        public SubgridOp() : this(0, 0) { } // for instantiating a generic type
    }
    public record OpList(ICollection<OpBase> ops) : OpBase, IEnumerable<OpBase>
    {
        public OpList() : this(new List<OpBase>()) { }
        public OpList(IEnumerable<OpBase> ops) : this(ops.ToList()) { }
        override public bool empty { get { return ops.Count == 0 || ops.All(op => op.empty); } }
        public IEnumerator<OpBase> GetEnumerator()
        {
            return ops.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(OpBase op)
        {
            ops.Add(op);
        }
    }
}
