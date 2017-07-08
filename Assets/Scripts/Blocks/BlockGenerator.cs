using System.Collections.Generic;
using System.Collections;
using UnityEngine;


/// [!] Notice: this MonoBehaviour might be used only by the server.
///     In this case this script should be disabled in all non-server player.
public class BlockGenerator : MonoBehaviour
{
    public Vector2[] targets;
    public GameObject[] targeters;
    public GameObject blockSource;
    
    public float delay; // Delay to checkout block creating.
    public enum GeneratingMode
    {
        GenerateWhenAble, // Try to create one.
        GenerateOne // If failed to create one, try another random location until successfully created.
    }
    GeneratingMode generatingMode = GeneratingMode.GenerateWhenAble;
    public int maxAttempt = 64; // max attempt allowed when using Generate-one Mode.
    
    public float distance; // Will not deploy any block if a block has a distance to target point smaller than this.
    
    
    void Start() 
    {
        
    }
    
    float t = 0f;
    void FixedUpdate()
    {
        t += Time.fixedDeltaTime;
        if(t > delay)
        {
            t -= delay;
            int ID = Mathf.FloorToInt(UnityEngine.Random.Range(0, targets.Length + targeters.Length));
            bool generated = false;
            int count = 0;
            do {
                if(ID < targets.Length)
                {
                    if(CanGenerate(targets[ID]))
                    {
                        Generate(targets[ID]);
                        generated = true;
                    }
                }
                else
                {
                    ID -= targets.Length;
                    if(CanGenerate(targeters[ID].transform.position))
                    {
                        Generate(targeters[ID].transform.position);
                        generated = true;
                    }
                }
                count++;
            } while(generatingMode == GeneratingMode.GenerateOne && !generated && count < maxAttempt);
        }
    }
    
    bool CanGenerate(Vector2 loc)
    {
        foreach(var i in GameObject.GetWithtag())
        return false;
    }
    
    void Generate(Vector2 loc)
    {
        /// The random type thing are done by the block itself.
        Instantiate(blockSource);
    }
}
