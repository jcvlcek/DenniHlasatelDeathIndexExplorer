using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Genealogy
{
    internal class TreeViewWrapper
    {
        #region Private members
        private TreeView mTree;
        #endregion

        #region Constructors
        public TreeViewWrapper(TreeView tvTarg)
        {
            mTree = tvTarg;
        }
        #endregion

        /// <summary>
        /// Called by the thread.start to set up for filling the DOM tree
        /// </summary>
        /// <param name="elemColl">HTMLElement collection with the "HTML" tag as its root</param>
        /// <remarks>Has to be passed as an object for the ParameterizedThreadStart</remarks>
        public void DisplayInTree(HtmlDocument docResponse)
        {
            HtmlElementCollection elemColl = docResponse.GetElementsByTagName("html");

            mTree.Nodes.Clear();
            TreeNode rootNode = new TreeNode("Web response");
            rootNode.Tag = -1;
            mTree.Nodes.Add(rootNode);
            FillDomTree(elemColl, rootNode, 0);
        }

        /// <summary>
        /// Recursive function to add the DOM element to the treeview
        /// </summary>
        /// <param name="currentEle">collection of elements</param>
        /// <param name="tn">tree node. currently not used</param>
        /// <remarks>The treenode was used prior to placing this function on its own thread
        /// at that point, I was nesting the nodes. right now, i am having trouble doing this across threads</remarks>
        private static void FillDomTree(HtmlElementCollection currentEle, TreeNode tn, int nodeTracker)
        {

            foreach (HtmlElement element in currentEle)
            {
                TreeNode tempNode;

                var tag = element.TagName;

                if (tag.CompareTo("!") == 0)
                {
                    tempNode = tn.Nodes.Add("<Comment>");
                }
                else
                {
                    string sNodeText = "<" + element.TagName;
                    if ((element.Name != null) && (element.Name.Length > 0))
                        sNodeText += " name=\"" + element.Name + "\"";
                    if ((element.Id != null) && (element.Id.Length > 0))
                        sNodeText += " id=\"" + element.Id + "\"";
                    sNodeText += ">";
                    tempNode = tn.Nodes.Add(sNodeText);
                    int iInnerTextLength = element.InnerText != null ? element.InnerText.Length : 0;
                    if (iInnerTextLength > 0)
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

        public void DisplayInTree(IObject oRoot)
        {
            FillTree(oRoot, null);
        }

        private void FillTree(IObject oTarg, TreeNode nodParent)
        {
            TreeNode tvNew = new TreeNode(oTarg.Name);
            if (nodParent == null)
                mTree.Nodes.Add(tvNew);
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
    }
}
