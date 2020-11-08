using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Random = System.Random;

public interface IRandom<out T> where T : unmanaged
{
    T Next();
}

public sealed unsafe class NativeRandom : IRandom<float>, IDisposable
{
    [StructLayout(LayoutKind.Sequential)]
    private struct Node
    {
        public Node* Next;
        public float Value;
    }

    private Node* m_MemBlock;
    private Node* m_Head;

    public NativeRandom(int count)
    {
        Random r = new Random();
        m_MemBlock = (Node*) Marshal.AllocHGlobal(sizeof(Node) * count);

        for (int i = 0; i < count - 1; i++)
        {
            m_MemBlock[i].Value = (float) r.NextDouble();
            m_MemBlock[i].Next = &m_MemBlock[i + 1];
        }

        m_MemBlock[count - 1].Next = m_MemBlock;
        m_Head = m_MemBlock;
    }

    ~NativeRandom()
    {
        Dispose();
    }

    public void Dispose()
    {
        if (m_MemBlock == null) return;
        Marshal.FreeHGlobal((IntPtr) m_MemBlock);
        m_MemBlock = null;
        m_Head = null;
    }

    public float Next()
    {
        m_Head = m_Head->Next;
        return m_Head->Value;
    }
}

public sealed class ManagedRandom : IRandom<float>
{
    private Node m_Head;

    private class Node
    {
        public Node Next;
        public float Value;
    }

    public ManagedRandom(int count)
    {
        Random r = new Random();
        m_Head = new Node();
        Node originalNode = m_Head;
        for (int i = 1; i < count; i++)
        {
            m_Head.Value = (float) r.NextDouble();
            m_Head.Next = new Node();
            m_Head = m_Head.Next;
        }

        m_Head.Next = originalNode;
        m_Head.Value = (float) r.NextDouble();
    }

    public float Next()
    {
        m_Head = m_Head.Next;
        return m_Head.Value;
    }
}