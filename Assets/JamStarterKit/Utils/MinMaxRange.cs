using UnityEngine;

namespace GameBase.Utils
{
    [System.Serializable]
    public class MinMaxRange
    {
        public MinMaxRange( ) {}
	
        public MinMaxRange( float val )
        {
            m_min = val;
            m_max = val;
            m_value = val;
            m_hasMax = false;
        }
	
        public MinMaxRange( float min, float max )
        {
            m_min = min;
            m_max = max;
            m_value = min;
            m_hasMax = true;
        }
	
        // Public for editor... damnit. there's gotta be a better way :(
        public float m_min = 0;
        public float m_max = 0;
        public bool m_hasMax = false;	
        public bool m_hasValue = false;
	
        float m_value = 0;
	
        public float Min { get { return m_min; } }
        public float Max { get { return m_hasMax ? m_max : m_min; } }
        public float Value { get { return (float)this; } }
        public float Lerp(float ratio) 
        { 
            if ( m_hasMax == false ) 
                return m_min; 
            return Mathf.Lerp(m_min, m_max, ratio); 
        }
	
        public static implicit operator float(MinMaxRange m) 
        {
            if ( m.m_hasValue == false )
            {		
                if ( m.m_hasMax )
                {
                    m.m_value = Random.Range(m.m_min, m.m_max);
                    m.m_hasValue = true;
                    return m.m_value;
                }
                else
                {
                    m.m_value = m.m_min;
                    m.m_hasValue = true;
                }
            }
            return m.m_value;
        }
	
        public static implicit operator int(MinMaxRange m) 
        {
            return Mathf.RoundToInt((float)m);
        }
	
        public void Randomise()
        {
	
            if ( m_hasMax )
            {
                m_value = Random.Range(m_min, m_max);
                m_hasValue = true;
            }
            else
            {
                m_value = m_min;
                m_hasValue = true;
            }
		
        }
	
        public float GetRandom()
        {
            if ( m_hasMax )
            {
                return Random.Range(m_min, m_max);
            }
            return m_min;		
        }
	
        public bool IsZero()
        {
            return m_min == 0 && m_hasMax == false;
        }
    }
}