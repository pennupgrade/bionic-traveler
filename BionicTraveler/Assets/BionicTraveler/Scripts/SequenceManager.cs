using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SequenceManager {
     // VARIABLES FROM CHART
     private List<GameObject> triggers; // these are assumed to be GameObject signals sent to 
                                        // request the initiation of some (list of) event(s)
     private List<ScriptedEvent> scriptedEvents;

     // ALTERNATIVE IDEAS
     /// <summary>
     /// A list of events that should be associated with each trigger 
     /// (events that should be triggered when the GameObject trigger is active)
     /// </summary>
     private List<List<ScriptedEvent>> eventLists;

     /// <summary>
     /// A list of pairs consisting of a GameObject (trigger) and a 
     /// list of corresponding ScriptedEvents that should start (be triggered)
     /// when that GameObject (trigger) is active
     /// </summary>
     private Dictionary<GameObject, List<ScriptedEvent>> triggersEventsPairs;  // maps each trigger to a corresponding list of associated events
     

     private bool eventPlaying;
     private List<bool> eventsPlayingStatus;
     
     /// <summary>
     /// Trigger the event, or list of events, associated 
     /// </summary>
     /// <param>GameObject signal to trigger corresponding events.</param>
     public void triggerEvent(GameObject trigger) {
        int v = triggers.IndexOf(trigger);
        int triggerIndex = v;

          // Trigger a single event (as indicated by chart)
          scriptedEvents[triggerIndex].PlaySequence();
          // OR
          // Trigger a list of events associated with the trigger signal (GameObject)
          foreach (ScriptedEvent eventToTrigger in eventLists[triggerIndex]) {
               eventToTrigger.PlaySequence();
          }
     }
     /// <summary>
     /// Check all triggers for status - whether or not they have been triggered
     /// uses .activeInHierarchy as the boolean indicator of whether a trigger (GameObject) has been activated
     /// </summary>
     public void update() {
          foreach (GameObject t in triggers) {
               if(t.activeInHierarchy) { 
                    triggerEvent(t);
               }
          }
     }

     

     

}