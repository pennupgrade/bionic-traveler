namespace BionicTraveler.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;
    using TMPro;

    /// <summary>
    /// Please document me.
    /// </summary>
    public class HandleTxt : MonoBehaviour
    {
        private TextMeshProUGUI fanficText;
        private String readInStream;
        private StreamReader reader;

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        public void Start()
        {
            fanficText = gameObject.GetComponent<TextMeshProUGUI>();
            
            pageTurn();
            
        }

        void pageTurn()
        {
            var text = @"Prologue:
The player sees a brightly colored painting of a man. The man has golden blonde hair and a porcelain smile. He is unblemished, unscarred and shaved clean. He holds his left arm against his chest, fist clenched.In his other hand he holds an assault rifle barrel pointing to the sky, and he wears a plain white shirt with the top buttons undone to expose part of his chest.Flanking him are soldiers dressed in camouflage, weapons drawn, kneeling, as if ready to engage a threat at moments notice.Fighter jets scream across the sky overhead.The soldiers and jets frame the man perfectly. An old timey voice begins: �Charlie Beck hero of the Continuation War, savior of humanity, paragon of our generation wants you. To join the fight. The Pestilence will one day return, but we will be ready.We will trade them in blood, we will trade them in bone, and they will once again learn that we have more than enough bullets to spare for their thick alien skulls.We may have won the war, but the fight for our survival is eternal�

The frame zooms in on Beck's face, and the scene shifts. The player sees another man, he has a gnarled scar over his left eye, he is sporting a graying beard with a well groomed head of graying hair to match. His expression is one of tiredness and apathy. The scene shifts and the player sees Beck staring out the window of an office. A woman enters. Beck acknowledges her and he asks about the first candidate: Dess


The player is introduced to Dess, she stands ready in a large room with concrete floors and steel walls.A PA system begins: �Candidate: 003, this is the combat assessment portion of your interview, please perform the tasks to the best of your physical and mental ability� The tutorial begins: The game teaches the player how to control Dess and fight with her.Once the tutorial ends, the scene changes once again to Beck.


Beck remarks, �Her combat assessment indicates she shows promise, what of her mental assessment ?� Beck�s assistant replies, stating that the candidate shows strong signs of neurological divergence and resistance to authority. �Shame,� Beck says, �The difference between a beast and a soldier is that a soldier follows orders.� The assistant simply begins reading about the next candidate.


A few days pass and Dess awakes in her bed, she mentions that she should break the bad news to Lyra.The player walks outside to the slums of Hestia.There is a soft drizzle.The environment is full of dull grays and browns and the only piece of color is that painting of Charlie Beck on a billboard, it is clear that it is a piece of propaganda. Walking down the street, there are many cameras scattered about monitoring the area. There are also numerous npcs scattered around that the player can talk to.By opening discussion, the npcs will either lament about the taste of rations, or talk about Miasma.Either they have it, or a family member has it.They may talk about how they don�t have symptoms, so it�s not that bad, or how they�re hoping to get their cybernetic cures soon. They will often express envy or solidarity with Dess, making some remark about her cybernetic arm.Notably, ending a conversation with an npc will cause them to say �Long may they govern�

Dess finally meets up with Lyra. Dess tells Lyra that she was passed over as a Presidium Guard. Lyra shrugs it off and jokingly says that would�ve made their job too easy anyway.She also tells Dess that they should be able to make due with Lyra�s benefits as a researcher.They exchange niceties and Lyra tells Dess where to find the shuttle. Lyra also explains that everything has been prepared for her.Dess promises that she will be quick, and Lyra jokes that she was only recently diagnosed with Miasma, and that Dess could take her sweet time.Dess doesn�t seem to take the joke well, Lyra reassures Dess that if anyone could build a cure, it was them. 

Dess departs on the shuttle, destined for a distant planet

Lyra briefs Dess on the planet that she travels to.The planet has a small Chancellery presence, and two distinct cultural factions that exist independently of each other with little contact.Officially the Chancellery is there for scientific observation and study of these two cultures.Lyra believes this, but Dess remarks with disdain that she does not believe that the Chancellery would have any interest in culture.Lyra then talks about how the Miasma appeared on the planet as soon as the Chancellery came, and she hopes that they could study how the disease spreads.


Dess lands on the planet and what follows is the vertical slice.


Wild Realm:
Whenever Dess interacts with NPCs in the Wild Realm.They react to her with amaze or disbelief.They call her sentinel.Many of them beg her to save them from the Scourge of the Forest.They tell Dess to find an old sage in one of the villages.


The old sage will tell Dess about a revered group of hunters called sentinels who kept the beasts of the forest at bay.She tells Dess that all had silver arms or legs just like her.Lyra speculates that it could've been a group of Chancellery soldiers that were stationed in the region. The sage will also talk about the wild beasts that inhibit the forests. She describes them as having an unrivaled anger and thirst for violence. Many of these beasts though, when undisturbed can be observed moving very slowly and carefully, as if their whole body ached. Dess asks where to find the Scourge of the Forest and the sage will provide some scout reports giving tis rough location


Upon finding and defeating the scourge Lyra will ask for a flesh sample.She will confirm her immediate suspicions that the bear was affected by Miasma.She makes note that though this is not unheard of for diseases to be virulent towards multiple species.Besides that she notes that the bear's biology seems to have been tampered with. She further extrapolates that her best guess is that the bear underwent some testing by the Chancellery and was released or escaped into the wild.


Fantasy Realm:
Compared to the wild realm, the people here react to Dess with much less interest.All people in this realm exhibit some psychic talent.Almost all of them walk around levitating their belongings.There is a noticeable class structure here based on psychic aptitude.Many of the poorer NPCS will lament that the tyrant�s tax collectors are too harsh, or that there isn�t enough food for the table.When pressed about the tyrant�s appearances, npcs note that most people never see him, and he rarely comes out. All they know of him are his harsh policies. (Also, the people are furries). 

Though the tyrant is cruel and his policies harsh, the people seem to revere him.They claim that he granted them their powers.

Dess can be directed to the tyrant�s palace. Where she could confront him.Dess initially does not see a reason for this, but Lyra convinces her that she wants to get a better understanding of the psychic abilities, as she has never seen anything like it.Dess worries that their time is short, but Lyra says that they have to exhaust all options when looking for a cure, as conventional means are not useful.

Dess relents and decides to at least talk with the tyrant.On the way to the palace, some guards try to arrest her for disturbing the peace on orders of the tyrant. She resists the arrest and makes note their psychic abilities are much more potent, but much of their body looks scarred and almost decaying. 

Dess enters the palace where after a series of confrontations she is face to face with the tyrant. He explains that their society is built on will and power, and that he rules because he is the strongest. Dess smiles and says, �we will see about that.� The tyrant assails Dess with his powerful psychic abilities, and he even has the power to control some lesser thralls.However, Dess prevails in the end. Severely weakening and incapacitating the tyrant.

It should be noted that the tyrant is missing his limbs, and exhibits severe scarring. Lyra notes that she suspects that their psychic abilities are somehow correlated with the severity of their Miasma infection.She asks Dess to scan the tyrant's corpse which she obliges. Dess notes that there is a piece of hardware at the back of the tyrant�s head. Dess removes it and the corpse decays significantly faster. 

It is clearly a piece of Chancellery technology, and Lyra suspects it is giving the people their psychic abilities. However, she does not understand the purpose of it being installed.
Sci-Fi Realm:
This realm has many scattered groups of Chancellery soldiers and scientists.They are all initially neutral, but Dess can attack them, making all groups hostile. Lyra will be upset if Dess does this, but won�t force Dess to stop.

The Chancellery will debrief Dess on the situation if they are not attacked, but otherwise Dess will have to explore the region without any guidance. One of the research installations was doing AI research.An experimental combat AI, went rogue and has taken over that installation. The Chancellery has confined it there, but they can�t shut it down. Dess will suggest destroying the installation, but any Chancellery scientist will say that it would be years of research being sacrificed.Whereas soldiers will jokingly wish Dess luck on her endeavor.

All the Chancellery NPCS here will say �Long may they govern� and do a Chancellery salute (left arm across chest with clenched fist) When a conversation is ended.Dess will also find many terminals or workstations where she can try and access Chancellery files on the research being done here.At all instances, she will be presented with redacted information, or she will be told that her clearance does not permit her to view classified documents. What information is available pertains to the Pestilence, Lyra suspects it was information fed to the AI.

Though Dess may initially set out to destroy the rogue AI installation. Lyra will note that a combat AI would have a very high clearance level. Dess will ask if this could be done remotely, and then Lyra will go on a very long and nerdy spiel about how and why Chancellery computer systems are designed to only allow attacks or infiltrations from the location of the AI or server (which will become relevant later). Dess will cut her off and go confront the AI.

After going to the installation and defeating the AI, Lyra will note that it is indeed strange that an AI would go rogue as it has never happened in the past. Dess will retort that it has never happened that they are aware of.
Ending:
After fully exploring the planet, Lyra will direct Dess to a ship repair terminal to refuel the shuttle.On the way there, Lyra will summarize what they learned. They know clearly that the Chancellery is not here on the planet simply for studying the local cultures. She manages to access some files confirming that the Chancellery has been conducting experiments on these societies.Lyra initially believes that the reason they have been hiding this information is that the Chancellery does not want to be seen as invaders, making them hypocrites.Dess will then solemnly say that despite all they have been through, they are no closer to a cure.Lyra optimistically will reply that they have learned so much more.She will note that she has also been decrypting some more highly classified files regarding the studies conducted on the planet.Lyra will reassure Dess once again, and tell her to get moving, and that they will talk again when she has more updates.

Upon landing on Hestia, Lyra will panically call Dess, more flustered than before. She will explain that Miasma is not some disease brought over by the Pestilence, but a piece of technology modeled off their society.The Miasma they see are essentially nanobots that behave and act like viruses.Being electrical in nature they can interface and interact with pieces of technology like a cybernetic implant or an AI.However, the nanobots are not themselves intelligent enough to do anything except consume and destroy.Which is why the Pestilence developed a centralized command computer for the Miasma modeled after their queens� brains.This supercomputer is housed at Chancellery headquarters. Like the drones of the Pestilence, the Miasma will all die if the queen dies.Lyra also notes that any technology that uses the Miasma, like cybernetics will be rendered useless should the supercomputer be destroyed.

Dess rushes to Lyra�s home where she is met with an embrace, and Lyra also slips something into Dess� pocket.The reunion is short lived as the Chancellery has already sent soldiers to arrest Dess and Lyra.TLDR Lyra gets captured because she is sick and not a super soldier, whereas Dess gets away.


Dess goes to Chancellery headquarters to save Lyra and to destroy the super computer. She encounters much resistance including from the Presidium Guard who taunt Dess a bit. However, they are quickly dispatched by her. She finally is able to confront the head of the Chancellery, Beck.Who calls her foolish for interfering and calls her a traitor to their species.Dess will ask if all this is worth it, and Beck will reply that sacrifices are necessary to win wars.They fight, and Beck makes use of the technologies developed on the other planet.


Once Dess defeats Beck, he claims that the Chancellery won�t end with him. There will always be good soldiers. Dess then uses the device Lyra slipped in her pocket and uses it.Dess remembers that Lyra mentioned that Chancellery systems need to be taken down with close proximity to the source.With that in mind, Dess uses the tech, and suddenly all the cybernetics ceases to work, as the supercomputer is destroyed.";

         fanficText.SetText(text);
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        public void Update()
        {
            //fanficText.SetText("Work damnit");
        }
    }
}
