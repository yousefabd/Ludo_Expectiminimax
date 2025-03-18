using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HashCodeTesting : MonoBehaviour
{
    class A
    {
        public int number;
        public bool check;
        public A(int number, bool check)
        {
            this.number = number;
            this.check = check;
        }
        public void DoSomething()
        {
            number++;
        }
        public override int GetHashCode()
        {
            return number;
        }
    }
    A a = new A(5, true);
    private void Start()
    {
        A b = new A(5, false);
        Debug.Log(a.GetHashCode());
        Debug.Log(b.GetHashCode());
    }
}
