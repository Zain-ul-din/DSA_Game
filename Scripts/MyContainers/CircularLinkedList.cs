/// Created By : Zain ul din  | On : 04/02/2022

/*
  * Feel free to change acording to you're requirements
*/
using System;
using System.Collections;
using System.Collections.Generic;



namespace CustomContainer {

    // @ Helper
    public sealed class Node<T>  {
     public T data;
     public Node<T> prev = null , next = null;

     public Node(T data) { 
         this.data = data;
         prev = null;
         next = null;
     }
    }
    
    
   // Custom Container
    public sealed class CircularLinkedList<T> {

        public delegate void MyEventHandler(ref Node<T> node);
        public event  MyEventHandler OnRemove;
        
        public delegate void NodeCallBack(Node<T> node);
        public delegate void CallBack(T data);
        
        public Node<T> head, tail;
        private int count;

      public CircularLinkedList() {
          head = null;
          tail = null;
      }

      /// <summary>
      /// Insert New Data on the top of  Linked List
      /// </summary>
      /// <param name="data">T newData</param>
      public void Push(T data) {
          Node<T> newNode = new Node<T>(data);
          // null case
          if (IsEmpty()) {
              head = newNode;
              tail = newNode;
              head.prev = tail;
              tail.next = head;
              count += 1;
              return;
          }

          newNode.next = head;
          newNode.prev = head.prev; // set tail

          head.prev = newNode;
          head = newNode;
          head.prev.next = head; // reset tail again
          count += 1;
      }
      
      /// <summary>
      /// Push Back data in Linked List
      /// </summary>
      /// <param name="data">T newData</param>
      public void Push_Back(T data) {
          Node<T> newNode = new Node<T>(data);
          // null case
          if (IsEmpty()) {
              head = newNode;
              tail = newNode;
              head.prev = tail;
              tail.next = head;
              count += 1;
              return;
          }
          
          // if not null
          newNode.prev = tail;
          newNode.next = tail.next; // set head 

          tail.next = newNode;
          tail = newNode;
          tail.next.prev = tail; //set head again
          count += 1;
      }
      
      /// <summary>
      /// Remove Given Element from Linked List
      /// </summary>
      /// <param name="data">T data</param>
      public void Remove(T data) {
         if(IsEmpty())  return;
         var itr = head;
         do {
             if (itr.data.Equals(data)) {
                 OnRemove(ref itr);
                 Remove_Helper(ref itr);
                 count -= 1;
                 return;
             } else itr = itr.next;
         } while (itr != head);
      }
      
      /// <summary>
      /// Returns LinkedList is Empty
      /// </summary>
      /// <returns>void</returns>
      bool IsEmpty() => head == null && tail == null;
      
      // ! node not be null
      private void Remove_Helper(ref Node<T> node) {

          if (node == head && node == tail) {
              head = null;
              tail = null;
              node = null;
              return;
          }

          if (node == head) {
              head = head.next;
              head.prev = tail;
              head.prev.next = head; // re set tail again
              return;
          }

          if (node == tail) {
              tail = tail.prev;
              tail.next = head;
              tail.next.prev = tail; // reset head again
              return;
          }

          if (node.next != null)
              node.next.prev = node.prev;
          if (node.prev != null)
              node.prev.next = node.next;

          node = null;
      }
      
      /// <summary>
      /// Allows to Iterate Over each Elements
      /// </summary>
      /// <param name="callBack"></param>
      public void ForEach(CallBack callBack) {
          if(IsEmpty())
              return;
         var itr = head;
          do {
              callBack(itr.data);
              itr = itr.next;
          } while (itr != head);
      }

      public void For_Each(NodeCallBack callBack) {
          if(IsEmpty())
              return;
          var itr = head;
          do {
              callBack(itr);
              itr = itr.next;
          } while (itr != head);
      }
      
      bool IsContain(T data) {
          if (IsEmpty())
              return false;
          var itr = head;
          do {
              if (itr.data.Equals(data))
                  return true;
              else itr = itr.next;
          } while (itr != head);
          return false;
      }
      
      /// <summary>
      /// Get Current Elements Count
      /// </summary>
      /// <returns></returns>
      public int GetCount() => this.count;
    } //  Class

}// Namespace
