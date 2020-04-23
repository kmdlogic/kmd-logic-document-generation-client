using System;
using System.Collections.Generic;
using System.Linq;

namespace Kmd.Logic.DocumentGeneration.Client.Configuration
{
    public class HierarchyPath
    {
        // A hierarchyPath is a sequence of strings.
        // The first, if it exists, must be empty.
        // No other entry can empty.
        // A hierarchyPath's string representation is \ separated.
        public HierarchyPath(IReadOnlyCollection<string> path)
        {
            if (path == null || path.Count == 0)
            {
                this._hierarchyPath = string.Empty;
                this._path = Array.Empty<string>();
            }
            else
            {
                this._path = path.ToArray();
                this._hierarchyPath = string.Join(Separator, this._path) + Separator;
            }
        }

        public HierarchyPath(string hierarchyPath)
        {
            if (string.IsNullOrEmpty(hierarchyPath))
            {
                this._hierarchyPath = string.Empty;
                this._path = Array.Empty<string>();
            }
            else
            {
                var hierarchyPathWithoutRoot = hierarchyPath.Trim().Trim(SeparatorChar);
                this._path = hierarchyPathWithoutRoot.Split(SeparatorChar).Prepend(string.Empty).ToArray();
                this._hierarchyPath = Separator + hierarchyPathWithoutRoot + Separator;
            }
        }

        public HierarchyPath Add(string segment)
        {
            return new HierarchyPath(this.ToPath().Append(segment).ToArray());
        }

        public HierarchyPath Tail => new HierarchyPath(this.ToPath().Skip(1).ToArray());

        public HierarchyPath ReplaceLeaf(string leaf)
        {
            var list = new List<string>();
            for (var index = 0; index < this._path.Length - 1; ++index)
            {
                list.Add(this._path[index]);
            }

            list.Add(leaf);
            return new HierarchyPath(list.ToArray());
        }

        public string Head => this.ToPath()[0];

        public int Length => this.ToPath().Length;

        public string[] ToPath()
        {
            return this._path;
        }

        public override string ToString()
        {
            return this._hierarchyPath;
        }

        private const string Separator = @"\";
        private const char SeparatorChar = '\\';
        private readonly string _hierarchyPath;
        private readonly string[] _path;
    }
}