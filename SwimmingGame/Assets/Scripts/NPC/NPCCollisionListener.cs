using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Making this script seperate from NPCOverworld because sometimes NPCOverworld is a child and can't get OnCollision events
public class NPCCollisionListener : MonoBehaviour
{
    private void OnCollisionStay(Collision other) {
        ContactPoint[] myContacts = new ContactPoint[other.contactCount];
        for(int i = 0; i < myContacts.Length; i++)
        {
            myContacts[i] = other.GetContact(i);

            GetComponentInChildren<NPCOverworld>().allHits.Add(myContacts[i]);
        }
    }

}
