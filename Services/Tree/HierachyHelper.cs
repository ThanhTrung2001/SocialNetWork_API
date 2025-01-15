using EnVietSocialNetWorkAPI.Model.Queries;

namespace EnVietSocialNetWorkAPI.Services.Tree
{
    public class HierachyHelper
    {
        public static List<OrganizeNodeQuery> BuildHierarchy(List<OrganizeNodeQuery> nodes)
        {
            // Find root nodes
            var rootNodes = nodes.Where(n => n.Parent_Id == null || n.Parent_Id == Guid.Empty).ToList();

            foreach (var rootNode in rootNodes)
            {
                AttachChildren(rootNode, nodes);
            }

            return rootNodes;
        }

        public static OrganizeNodeQuery BuildHierarchyChild(List<OrganizeNodeQuery> nodes, Guid id)
        {
            // Find root nodes
            var rootNode = nodes.FirstOrDefault(n => n.Id == id);

            AttachChildren(rootNode, nodes);

            return rootNode;
        }

        public static void AttachChildren(OrganizeNodeQuery parentNode, List<OrganizeNodeQuery> allNodes)
        {
            var children = allNodes.Where(n => n.Parent_Id == parentNode.Id).ToList();
            parentNode.Children_Nodes = children;

            foreach (var child in children)
            {
                AttachChildren(child, allNodes);
            }
        }
    }
}
