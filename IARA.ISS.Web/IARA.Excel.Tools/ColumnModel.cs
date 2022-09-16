using System;
using System.Collections.Generic;
using System.Linq;

namespace IARA.Excel.Tools
{
    internal class ColumnModel : ICloneable
    {
        internal const byte A_ASCII_CODE = 65;
        internal const byte Z_ASCII_CODE = 90;

        private LinkedList<ColumnPart> columnParts;
        private IEnumerator<string> columnEnumerator;
        private List<string> columnNames;
        private bool done = false;

        public ColumnModel()
        {
            this.columnNames = new List<string>();
            this.columnParts = new LinkedList<ColumnPart>();
        }

        public string GetMaxColumnName()
        {
            return columnNames.Last();
        }

        public string GetNextColumnName()
        {
            if (done)
            {
                if (columnEnumerator.MoveNext())
                {
                    return columnEnumerator.Current;
                }
                else
                {
                    ResetColumns();
                    return GetNextColumnName();
                }
            }
            else
            {
                return GenerateNextColumnName();
            }
        }

        public void IndentColumns(int indent)
        {
            while (indent > 0)
            {
                GetNextColumnName();
                indent--;
            }
        }

        private string GenerateNextColumnName()
        {
            if (this.columnParts.Count == 0 || !this.AnyLeft())
            {
                this.columnParts.AddLast(new ColumnPart());
                this.columnParts.All(x => { x.Reset(); return true; });
            }

            var stack = new Stack<ColumnPart>(this.columnParts);

            while (!stack.Peek().HasNext())
            {
                stack.Pop();
            }

            ColumnPart currentItem = stack.Pop();

            currentItem.NextChar();

            LinkedListNode<ColumnPart> nextNode = columnParts.Find(currentItem).Next;

            while (nextNode != null)
            {
                nextNode.Value.Reset();
                nextNode = nextNode.Next;
            }

            string columnName = new(columnParts.Select(x => x.Character).ToArray());
            columnNames.Add(columnName);

            return columnName;
        }

        private bool AnyLeft()
        {
            foreach (ColumnPart part in new Stack<ColumnPart>(columnParts))
            {
                if (part.HasNext())
                {
                    return true;
                }
            }

            return false;
        }

        public void ResetColumns()
        {
            if (!done)
            {
                done = true;
                columnEnumerator = columnNames.GetEnumerator();
            }
            else
            {
                columnEnumerator.Reset();
            }
        }

        public object Clone()
        {
            var columns = new ColumnModel();

            columns.done = this.done;
            columns.columnNames = this.columnNames;
            columns.columnParts = this.columnParts;
            columns.columnEnumerator = this.columnEnumerator;

            return columns;
        }
    }

    internal class ColumnPart
    {
        public ColumnPart()
        {
            this.Reset();
        }

        private char character;
        public char Character
        {
            get
            {
                if (ASCIIRepresentation < ColumnModel.A_ASCII_CODE)
                {
                    this.NextChar();
                }

                return character;
            }
            private set
            {
                character = value;
            }
        }
        public byte ASCIIRepresentation { get; private set; }

        public byte NextChar()
        {
            this.ASCIIRepresentation++;
            this.character++;

            return this.ASCIIRepresentation;
        }

        public bool HasNext()
        {
            return this.ASCIIRepresentation + 1 <= ColumnModel.Z_ASCII_CODE;
        }

        public void Reset()
        {
            this.ASCIIRepresentation = ColumnModel.A_ASCII_CODE - 1;
            this.character = Convert.ToChar(ColumnModel.A_ASCII_CODE - 1);
        }
    }
}
