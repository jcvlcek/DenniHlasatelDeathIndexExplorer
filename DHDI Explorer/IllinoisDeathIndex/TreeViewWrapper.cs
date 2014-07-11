using System.Windows.Forms;

namespace Genealogy
{
    /// <summary>
    /// Wraps a tree view control with utilities to display
    /// an HTML document and/or an object tree
    /// </summary>
    internal class TreeViewWrapper
    {
        #region Private members

        /// <summary>
        /// The underlying tree view control
        /// </summary>
        private readonly TreeView _treeView;
        #endregion

        #region Constructors

        /// <summary>
        /// Wrap a tree view control to enable displaying
        /// HTML documents and/or object trees
        /// </summary>
        /// <param name="tvTarg">the tree view to wrap</param>
        public TreeViewWrapper(TreeView tvTarg)
        {
            _treeView = tvTarg;
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Display an HTML document in the tree view, organizing the tree
        /// in accordance with the Document Object Model (DOM)
        /// </summary>
        /// <param name="docResponse">the document to display</param>
        /// <remarks><para>Called by the thread.start to set up for filling the DOM tree</para>
        /// Has to be passed as an object for the ParameterizedThreadStart</remarks>
        public void DisplayInTree(HtmlDocument docResponse)
        {
            HtmlElementCollection elemColl = docResponse.GetElementsByTagName("html");

            _treeView.Nodes.Clear();
            var rootNode = new TreeNode("Web response") {Tag = -1};
            _treeView.Nodes.Add(rootNode);
            FillDomTree(elemColl, rootNode, 0);
        }

        /// <summary>
        /// Display an object containing hierarchically-structured data
        /// in the tree view
        /// </summary>
        /// <param name="oRoot"></param>
        public void DisplayInTree(IObject oRoot)
        {
            FillTree(oRoot, null);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Recursive function to add the DOM element to the treeview
        /// </summary>
        /// <param name="currentEle">collection of elements</param>
        /// <param name="tn">tree node. currently not used</param>
        /// <param name="nodeTracker">incremental count of the nodes in the tree</param>
        /// <remarks>The treenode was used prior to placing this function on its own thread
        /// at that point, I was nesting the nodes. right now, i am having trouble doing this across threads</remarks>
        private static void FillDomTree(HtmlElementCollection currentEle, TreeNode tn, int nodeTracker)
        {

            foreach (HtmlElement element in currentEle)
            {
                TreeNode tempNode;

                var tag = element.TagName;

                if (System.String.Compare(tag, "!", System.StringComparison.Ordinal) == 0)
                {
                    tempNode = tn.Nodes.Add("<Comment>");
                }
                else
                {
                    string sNodeText = "<" + element.TagName;
                    if (!string.IsNullOrEmpty(element.Name))
                        sNodeText += " name=\"" + element.Name + "\"";
                    if (!string.IsNullOrEmpty(element.Id))
                        sNodeText += " id=\"" + element.Id + "\"";
                    sNodeText += ">";
                    tempNode = tn.Nodes.Add(sNodeText);
                    int iInnerTextLength;
                    if ((element.InnerText != null ) && ((iInnerTextLength = element.InnerText.Length) > 0))
                    {
                        string sInnerText;
                        if (iInnerTextLength > 20)
                            sInnerText = element.InnerText.Substring(0, 20) + "...";
                        else
                            sInnerText = element.InnerText;
                        tempNode.Nodes.Add("InnerText=\"" + sInnerText + "\"");
                    }
                }

                /** a counter to keep track of the absolute index of the nodes. Stuffing value into tag **/
                tempNode.Tag = nodeTracker++;

                /** over and over and over and over **/
                FillDomTree(element.Children, tempNode, nodeTracker);
            }
        }

        /// <summary>
        /// Construct a graph of tree view nodes, following the
        /// hierarchical organization of descendants beneath a top-level object
        /// </summary>
        /// <param name="oTarg">the top-level object the tree will model</param>
        /// <param name="nodParent">the top-level tree node, to associate with <paramref name="oTarg"/></param>
        private void FillTree(IObject oTarg, TreeNode nodParent)
        {
            var tvNew = new TreeNode(oTarg.Name);
            if (nodParent == null)
                _treeView.Nodes.Add(tvNew);
            else
                nodParent.Nodes.Add(tvNew);

            for (int i = 0; i < oTarg.PropertyCount; ++i)
            {
                IProperty pNext = oTarg.GetProperty(i);
                tvNew.Nodes.Add(pNext.Name + " = " + pNext.Value);
            }

            for (int i = 0; i < oTarg.ChildCount; ++i)
                FillTree(oTarg.GetChild(i), tvNew);
        }

        #endregion
    }
}
