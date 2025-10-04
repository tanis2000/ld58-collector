namespace PowerTools
{
    public struct BitMask
    {	
        public BitMask( int mask )
        {
            m_value = mask;
        }

	
        public BitMask( params int[] bitsSet )
        {
            m_value = 0;
            for( int i = 0; i< bitsSet.Length; ++i )
            {
                m_value |= 1 <<  bitsSet[i];
            }
        }
	
        public static implicit operator int(BitMask m) 
        {
            return m.m_value;	
        }
		
        public int Value { get{ return m_value; } set { m_value = value; } }
        public void SetAt(int index) { m_value |= 1 << index; }
        public void SetAt<T>(T index) { m_value |= 1 << (int)(object)index; }
        public void UnsetAt(int index) { m_value &= ~(1 << index); }
        public void UnsetAt<T>(T index) { m_value &= ~(1 << (int)(object)index); }
        public bool IsSet(int index) { return (m_value & 1 << index) != 0; }
        public bool IsSet<T>(T index) { return (m_value & 1 << (int)(object)index) != 0; }
        public void Clear() { m_value = 0; }
	
        // And some static functions if you don't wanna construt the bitmask  and just wanna pass in/out an int
        public static int SetAt(int mask, int index) { return mask | 1 << index; }
        public static int UnsetAt(int mask, int index)  { return mask & ~(1 << index); }
        public static bool IsSet(int mask, int index) { return (mask & 1 << index) != 0; }


        public static uint GetNumberOfSetBits(uint i)
        {
            // From http://stackoverflow.com/questions/109023/how-to-count-the-number-of-set-bits-in-a-32-bit-integer
            i = i - ((i >> 1) & 0x55555555);
            i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
            return (((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
        }

        int m_value;


    }
}