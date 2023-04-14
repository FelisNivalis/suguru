using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuLib.Strategy.Op
{
    public interface OpBase { }
    public record struct OpList(ICollection<OpBase> ops) : OpBase, IEnumerable<OpBase>
    {
        public OpList() : this(new List<OpBase>()) { }
        public OpList(IEnumerable<OpBase> ops) : this(ops.ToList()) { }
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
    public record struct DigitSelectOp(int Row, int Column, int Digit) : OpBase { }
    public record struct SubgridSelectOp(int Row, int Column) : OpBase { }
    public record struct DigitUnselectOp(int Row, int Column, int Digit) : OpBase { }
    public record struct SubgridUnselectOp(int Row, int Column) : OpBase { }
    public record struct SubgridHLErrorOp(int Row, int Column) : OpBase { }
    public record struct SubgridHLDefaultOp(int Row, int Column) : OpBase { }
    public record struct EliminateOp(int Row, int Column, int Digit) : OpBase { }
    public record struct UnEliminateOp(int Row, int Column, int Digit) : OpBase { }
    public record struct FillOp(int Row, int Column, int Digit) : OpBase { }
}
