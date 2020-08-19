using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DatumCollection
{
    // Definition for a binary tree node.
    public class TreeNode
    {
        public int val;
        public TreeNode left;
        public TreeNode right;
        public TreeNode(int x) { val = x; }
    }

    #region binary tree
    //public class Solution
    //{
    //    //1.递归
    //    //public int Rob(TreeNode root)
    //    //{
    //    //    if(root == null)
    //    //    {
    //    //        return 0;
    //    //    }

    //    //    int sum = root.val;

    //    //    if (root.left != null)
    //    //    {
    //    //        sum += Rob(root.left.left) + Rob(root.left.right);
    //    //    }

    //    //    if(root.right != null)
    //    //    {
    //    //        sum += Rob(root.right.left) + Rob(root.right.right);
    //    //    }

    //    //    return Math.Max(sum, Rob(root.left) + Rob(root.right));
    //    //}

    //    /*
    //     * using a dictionary to cache the node sum value
    //     * no more duplicate calculation
    //     */
    //    //public int Rob(TreeNode root)
    //    //{
    //    //    var cache = new Dictionary<TreeNode, int>();
    //    //    return RobWithMemory(root, cache);
    //    //}

    //    //int RobWithMemory(TreeNode node, Dictionary<TreeNode,int> dict)
    //    //{
    //    //    if (node == null)
    //    //    {
    //    //        return 0;
    //    //    }

    //    //    //if the current root is calculated
    //    //    if (dict.ContainsKey(node))
    //    //    {
    //    //        return dict[node];
    //    //    }

    //    //    int sum = node.val;

    //    //    if (node.left != null)
    //    //    {
    //    //        sum += RobWithMemory(node.left.left, dict) + RobWithMemory(node.left.right, dict);
    //    //    }

    //    //    if(node.right != null)
    //    //    {
    //    //        sum += RobWithMemory(node.right.left, dict) + RobWithMemory(node.right.right, dict);
    //    //    }

    //    //    int result = Math.Max(sum, RobWithMemory(node.left, dict) + RobWithMemory(node.right, dict));
    //    //    dict.Add(node, result);

    //    //    return result;
    //    //}


    //    /*
    //     * when node is selected or not,return an array of sum and return the larger one
    //     */
    //    public int Rob(TreeNode root)
    //    {
    //        int[] result = RobInternal(root);
    //        return Math.Max(result[0], result[1]);
    //    }

    //    public int[] RobInternal(TreeNode node)
    //    {
    //        int[] result = new int[2];
    //        if (node == null)
    //        {
    //            return result;
    //        }

    //        int[] left = RobInternal(node);
    //        int[] right = RobInternal(node);

    //        result[0] = Math.Max(left[0], left[1]) + Math.Max(right[0], right[1]);
    //        result[1] = left[0] + right[0] + node.val;

    //        return result;
    //    }
    //}
    #endregion

    //public class Foo
    //{

    //    public Foo()
    //    {

    //    }

    //    private SpinWait _wait = new SpinWait();
    //    private int _num = 1;

    //    public void First(Action printFirst)
    //    {

    //        // printFirst() outputs "first". Do not change or remove this line.
    //        printFirst();
    //        Thread.VolatileWrite(ref _num, 2);
    //    }

    //    public void Second(Action printSecond)
    //    {
    //        while (Thread.VolatileRead(ref _num) != 2)
    //        {
    //            _wait.SpinOnce();
    //        }
    //        // printSecond() outputs "second". Do not change or remove this line.
    //        printSecond();
    //        Thread.VolatileWrite(ref _num, 3);
    //    }

    //    public void Third(Action printThird)
    //    {
    //        while (Thread.VolatileRead(ref _num) != 3)
    //        {
    //            _wait.SpinOnce();
    //        }
    //        // printThird() outputs "third". Do not change or remove this line.
    //        printThird();
    //    }
    //}

    //public class Solution
    //{
    //    public int[] TwoSum(int[] nums, int target)
    //    {
    //        int[] indices = new int[2];
    //        if (nums == null || nums.Length == 0)
    //        {
    //            return null;
    //        }

    //        Dictionary<int, int> dic = new Dictionary<int, int>();
    //        for (int i = 0; i < nums.Length; i++)
    //        {
    //            var rest = target - nums[i];
    //            if (dic.ContainsKey(rest))
    //            {
    //                return new int[] { i, dic[rest] };
    //            }
    //            if (!dic.ContainsKey(nums[i]))
    //            {
    //                dic.Add(nums[i], i);
    //            }                
    //        }
    //        return indices;
    //    }
    //}

    
  //Definition for singly-linked list.
  public class ListNode {
      public int val;
      public ListNode next;
      public ListNode(int x) { val = x; }
  }

    public class Solution
    {
        public ListNode AddTwoNumbers(ListNode l1, ListNode l2)
        {
            int total = CalculateSum(l1) + CalculateSum(l2);
            return IntToListNode(total);
        }

        int CalculateSum(ListNode list)
        {
            int num = 0;
            int iteration = 0;
            var current = list;
            while (current != null)
            {
                num += current.val * 10 ^ iteration;
                current = current.next;
                iteration++;
            }

            return num;
        }

        ListNode IntToListNode(int input)
        {
            var inputArray = input.ToString().Reverse().ToArray();
            if(inputArray.Length == 0) { return null; }
            var list = new ListNode(inputArray[0]);
            var current = list;
            for (int i = 0; i < inputArray.Length; i++)
            {
                current.next = new ListNode(inputArray[i]);
            }

            return list;
        }
    }
}
