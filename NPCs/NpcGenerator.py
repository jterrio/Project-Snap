import math
import random
import xml.etree.ElementTree as ET
archetype = ""
intuition = 0
intelligence = 0
strength = 0
charisma = 0
precision = 0
dexterity = 0
perception = 0
canUseMagic = False
weaponType = ["Gun","Sword","GunSword","GunMage","MageSword","Magic"]
favWeapon = "Nothing"
physicalTraits = 0 #made later
personalityTraits = 0 #made later

def ExportToXML(archetype, canUseMagic, favWeapon, physicalTraits, personalityTraits):
    global intuition
    global intelligence
    global strength
    global charisma
    global precision
    global dexterity
    global perception
    #create the file structure
    npc = ET.Element('npc')
    
    attributes = ET.SubElement(npc,'attributes')
    arch = ET.SubElement(attributes, 'archetype')
    magic = ET.SubElement(attributes, 'canUseMagic')
    weapon = ET.SubElement(attributes, 'favWeapon')
    
    #setting the values
    arch.text = str(archetype)
    magic.text = str(canUseMagic)
    weapon.text = str(favWeapon)
    
    stats = ET.SubElement(npc, 'stats')
    intu = ET.SubElement(stats, 'intuition')
    intel = ET.SubElement(stats, 'intelligence')
    stren = ET.SubElement(stats, 'strength')
    char = ET.SubElement(stats, 'charisma')
    pre = ET.SubElement(stats, 'precision')
    dex = ET.SubElement(stats, 'dexterity')
    per = ET.SubElement(stats, 'perception')

    #setting the values
    intu.text = str(intuition)
    intel.text = str(intelligence)
    stren.text = str(strength)
    char.text = str(charisma)
    pre.text = str(precision)
    dex.text = str(dexterity)
    per.text = str(perception)

    physical = ET.SubElement(npc,'physicalTraits')
    personality = ET.SubElement(npc,'personalityTraits')

    count = 0
    #Exporting Personality to XML
    for x in personalityTraits:
        if(x != 0 and x != "Average"):
            traits = ET.SubElement(physical,'traits')
            traits.text = str(x)

    
    #Exporting Physical to XML
    for x in physicalTraits:
        count += 1
        if(x != 0 and x != "Average"):
            if(count == 6):
                trait = ET.SubElement(personality,'traits')
                trait.text = str(x + "Hair")
                
            trait = ET.SubElement(personality,'traits')
            trait.text = str(x)

    # create a new XML file with the results
    mydata = ET.tostring(npc)  
    myfile = open("NpcGenerated.xml", "wb")  
    myfile.write(mydata)
    
    
    

def Character(startingStat,startingPer):
    global intuition
    global intelligence
    global strength
    global charisma
    global precision
    global dexterity
    global perception
    #und - ability to pickup skills in combat
    intuition = startingStat
    #int - ability to study skills outside of combat
    intelligence = startingStat
    #str - ability to swing hard for physical attacks
    strength = startingStat
    #chr - ability to get people to like you
    charisma = startingStat
    #prc - ability to aim
    precision = startingStat
    #spr - ability to survive more attacks
    spirituality = 0
    #dex - ability to use more attacks or move
    dexterity = startingStat
    #per - ability to process info in combat (default time per turn is 0.75seconds. every level reduces this by 0.025, with a hard cap at 20 points)
    perception = startingPer 
        
#Updates Intuition
def updateUnd(updatedValue):
    global intuition
    intuition +=updatedValue
        
        #Updates Intelligence
def updateInt( updatedValue):
    global intelligence
    intelligence +=updatedValue
        
            #Updates Strength
def updateStr( updatedValue):
    global strength
    strength +=updatedValue
        
            #Updates Charisma
def updateChr( updatedValue):
    global charisma
    charisma +=updatedValue
        
            #Updates Precision
def updatePrc( updatedValue):
    global precision
    precision +=updatedValue
        
            #Updates Spirituality
def updateSpr( updatedValue):
    global spirituality
    spirituality +=updatedValue
        
            #Updates Dexterity
def updateDex( updatedValue):
    global dexterity
    dexterity +=updatedValue
        
            #Updates Perception
def updatePer( updatedValue):
    global perception
    perception +=updatedValue
        
            #Checks to see if the value is greater than 100 or less than 0 and corrects it
def CheckMaxMin( valueToCheck):
    if(valueToCheck > 100):
        return 100
            
    elif(valueToCheck < 0):
        return 0
            
    else:
        return valueToCheck

def FinalizeValues():
    global intuition
    global intelligence
    global strength
    global charisma
    global precision
    global dexterity
    global perception
    intuition = CheckMaxMin(intuition)
    intelligence = CheckMaxMin(intelligence)
    strength = CheckMaxMin(strength)
    charisma = CheckMaxMin(charisma)
    precision = CheckMaxMin(precision)
    dexterity = CheckMaxMin(dexterity)
    if(perception > 20):
        perception = 20

    elif(perception < 0):
        perception = 0
                
            
        
    

top = 85
midBot = 16
#returns a number bewtween 1 and 100
def RandomNum():
    return math.floor(random.randint(1,100))

#returns what type of npc they are going to be and the starting stats
def NpcType():
    global physicalTraits
    global personalityTraits
    global archetype
    global canUseMagic
    global weaponType
    global favWeapon
    student = ["Mage Student","Study Magic Student","Jock Sudent","Intellectual Student","Student"]
    rando = RandomNum()
    #Goverment People
    if(rando >= 80):
        Character(45,9)
        archetype = "Government Person"
        rando = RandomNum()
        #Can they use magic
        if(rando > 15):
            canUseMagic = True

        else:
            canUseMagic = False
        #print("Goverment Person", end = '\n')
            
    #Students: Mages, Non Mage but study magic, don't study magic
    elif(rando < 80 and rando > 60):
        rando = RandomNum()
        Character(40,8)
        #Mages
        if(rando >= 60):
            updateUnd(5)
            updateInt(-5)
            updateStr(5)
            updateDex(5)
            updatePrc(5)
            updatePer(2)
            archetype = student[0]
            canUseMagic = True
            
        #NonMage but sudy magic
        elif(rando < 55 and rando >= 35):
            updateUnd(-5)
            updateInt(5)
            updatePrc(3)
            updatePer(2)
            archetype = student[1]
            canUseMagic = False

        #Don't study magic
        else:
            rando = RandomNum()
            #Can they use magic
            if(rando > 30):
                canUseMagic = True

            else:
                canUseMagic = False

            rando = RandomNum()
            #Jock
            if(rando >= 60):
                updateStr(5)
                updateDex(5)
                updatePrc(5)
                updatePer(1)
                archetype = student[2]

            #Intellectual Study
            elif(rando < 60 and rando >= 40):
                updateUnd(5)
                updateInt(5)
                updatePer(2)
                archetype = student[3]

            #General Studies
            else:
                updateUnd(2)
                updateInt(2)
                updateStr(2)
                updateDex(2)
                updatePrc(2)
                archetype = student[4]
                
    #People on the streets
    elif(rando <= 60 and rando > 5):
        rando = RandomNum()
        #Can they use magic
        if(rando > 30):
            canUseMagic = True

        else:
            canUseMagic = False

        rando = RandomNum()
        #Average Person
        if(rando >= 60):
            People = ["Laborer","Soldier","Thug","BlackSmith","Banker"]
            Character(30,7)
            rando = RandomNum()
            #Laborer
            if(rando > 80):
               archetype = People[0]

            #Soldier
            elif(rando >= 80 and rando < 60):
                archetype = People[1]

            #Thug
            elif(rando >= 60 and rando < 40):
                archetype = People[2]

            #BlackSmith
            elif(rando >= 40 and rando < 20):
                archetype = People[3]

            #Banker
            else:
                archetype = People[3]
                
                

        elif(rando < 60 and rando >= 40):
            Character(50,7)
            updateStr(-5)
            archetype = "Veteran"

        elif(rando < 40 and rando >= 20):
            Character(10,4)
            archetype = "Child"
            
        #Lower Class
        else:
            Character(20,4)
            archetype = "Lower Class"
            
    #Hero Type
    else:
        Character(65,13)
        archetype = "Hero"
        rando = RandomNum()
        if(rando > 50):
            canUseMagic = True

        else:
            canUseMagic = False

    #Determine their fighting style
    rando = RandomNum()
    if(canUseMagic):
        #Gun Mage
        if(rando >= 66):
            updatePrc(5)
            updatePer(1)
            favWeapon = weaponType[3]

        #Sword Mage
        elif(rando < 66 and rando > 33):
            updateStr(5)
            updatePer(1)
            favWeapon = weaponType[4]

        #Mage   
        else:
            updateStr(3)
            updateDex(3)
            updatePrc(3)
            updatePer(1)
            favWeapon = weaponType[5]

    else:
        #Gun
        if(rando >= 66):
            updatePrc(5)
            favWeapon = weaponType[0]

        #Sword
        elif(rando < 66 and rando > 33):
            updateStr(5)
            favWeapon = weaponType[1]

        #GunSword
        else:
            updatePrc(3)
            updateStr(3)
            favWeapon = weaponType[2]


#creates an array of atributes to define an npc
def Atributes():
    global top
    global midBot
    global physicalTraits
    global archetype
    atributes = [0] * 20
    for x in atributes:
        atributes[x] = "0"
        
    rando = RandomNum()
    #What their words mean
    if(rando >= top):
        atributes[0] = "Genuine"
        updateChr(2)
    
    elif(rando < top and rando > midBot):
        atributes[0] = "Average"
    
    else:
        atributes[0] = "Ingenuine"
        updatePer(1)
        updateChr(-4)
    
    #Their Creativity
    rando = RandomNum()
    if archetype in("Government Person", "Mage Student","Intellectual Student","Study Magic Student"):
        rando += 10
        
    if(rando >= top):
        atributes[1] = "Creative"
        updateChr(2)
        updateInt(4)
        updateUnd(4)
    
    elif(rando < top and rando > midBot):
        atributes[1] = "Average"
        updateUnd(1)
        updateInt(1)
    
    else:
        atributes[1] = "Uncreative"
        updateInt(-4)
        updateUnd(-4)
        updateChr(-2)
    
    #Their Humor meter
    rando = RandomNum()
    if(rando >= top):
        atributes[2] = "Jokester"
        updateChr(3)
        updatePrc(1)
        updateDex(2)
    
    elif(rando < top and rando > midBot):
        atributes[2] = "Average"
    
    else:
        atributes[2] = "Serious"
        updatePrc(3)
        updateStr(1)
        updateChr(-3)
    
    #The way they hold themselves
    rando = RandomNum()
    if(physicalTraits[1] == "Buff"):
        rando += 15
        
    if(rando >= top):
        atributes[3] = "Arrogant"
        updateChr(-3)
        updatePrc(2)
    
    elif(rando < top and rando > midBot):
        atributes[3] = "Average"
        updateChr(1)
    
    else:
        atributes[3] = "SelfDepricating"
        updateInt(2)
        updateChr(-1)
    
    #How they think, body or mind, think how they react to things
    rando = RandomNum()
    if archetype in("Intellectual Student", "Hero", "Government Person", "Study Magic Student"):
        rando -= 10

    if(archetype == "Jock Sudent" or "Lower Class"):
        rando += 10

    if(physicalTraits[1] == "Buff"):
        rando += 15
        
    if(rando >= top):
        atributes[4] = "Meat-Head"
        updateUnd(10)
        updateInt(-5)
        updatePer(1)
        updateStr(2)
    
    elif(rando < top and rando > midBot):
        atributes[4] = "Average"
    
    else:
        atributes[4] = "Savvy"
        updateInt(10)
        updateUnd(-5)
        updatePer(1)
        updatePrc(2)
    
    #How they learn, via experience or books
    rando = RandomNum()
    if archetype in("Intellectual Student", "Study Magic Student"):
        rando += 10

    if archetype in("Mage Student", "Jock Sudent", "Lower Class"):
        rando -= 10

    if(physicalTraits == "Buff"):
        rando -= 20
        
    if(rando >= top):
        atributes[5] = "Book-Worm"
        updateInt(4)
        updatePrc(2)
        updateStr(-2)
    
    elif(rando < top and rando > midBot):
        atributes[5] = "Average"
    
    else:
        atributes[5] = "Hands-On"
        updateUnd(4)
        updateStr(2)
        updatePrc(-2)
        
    
    #Can they follow orders
    rando = RandomNum()
    if archetype in("Lower Class"):
        rando -= 10

    if archetype in("Government Person", "Mage Student"):
        rando += 10
        
    if(rando >= top):
        atributes[6] = "Obedient"
        updatePrc(3)
    
    elif(rando < top and rando > midBot):
        atributes[6] = "Average"
    
    else:
        atributes[6] = "Disobedient"
        updateDex(3)
    
    #Can they lead
    rando = RandomNum()
    if(rando >= top):
        atributes[7] = "Direct"
        updateChr(10)
    
    elif(rando < top and rando > midBot):
        atributes[7] = "Average"
    
    else:
        atributes[7] = "Unclear"
        updateChr(-10)
    
    #Can they Commit to things
    rando = RandomNum()
    if(rando >= top):
        atributes[8] = "Committed"
        updateInt(4)
    
    elif(rando < top and rando > midBot):
        atributes[8] = "Average"
    
    else:
        atributes[8] = "Procrastinator"
        updateInt(-4)
    
    #Thinking speed
    rando = RandomNum()
    if archetype in("Intellectual Student", "Government Person", "Study Magic Student"):
        rando += 20
        
    if(rando >= top):
        atributes[9] = "Quick-Thinker"
        updateInt(5)
        updateUnd(5)
        updatePrc(3)
        updateChr(3)
        updatePer(2)
    
    elif(rando < top and rando > midBot):
        atributes[9] = "Average"
    
    else:
        atributes[9] = "Slow"
        updateInt(-5)
        updateUnd(-5)
        updatePrc(-3)
        updateChr(-3)
        updatePer(-2)
    
    #How Loyal they can be
    rando = RandomNum()
    if(rando >= top):
        atributes[10] = "Loyal"
    
    elif(rando < top and rando > midBot):
        atributes[10] = "Average"
    
    else:
        atributes[10] = "Unloyal"
    
    #How easily they are persuaded to do something
    rando = RandomNum()
    if(rando >= top):
        atributes[11] = "Push-Over"
        updateDex(4)
    
    elif(rando < top and rando > midBot):
        atributes[11] = "Average"
    
    else:
        atributes[11] = "Stubborn"
        updateChr(-4)
        updatePer(1)
    
    #Their Intelligence
    rando = RandomNum()
    if archetype in("Intellectual Student", "Hero", "Government Person", "Study Magic Student"):
        rando += 10

    if archetype in("Lower Class", "Jock Sudent"):
        rando -= 10

    if(atributes[9] == "Slow"):
        rando -= 20
        
    if(rando >= top):
        atributes[12] = "Intelligent"
        updateInt(5)
        updateUnd(3)
    
    elif(rando < top and rando > midBot):
        atributes[12] = "Average"
    
    else:
        atributes[12] = "Unintelligent"
        updateInt(-5)
        updateUnd(-3)
    
    #Their reluctance to leave home
    rando = RandomNum()
    if archetype in("Lower Class", "Hero", "Mage Student"):
        rando += 10
        
    if(rando >= top):
        atributes[13] = "Wonderlust"
        updateDex(4)
        updateStr(4)
        updatePer(-1)
    
    elif(rando < top and rando > midBot):
        atributes[13] = "Average"
    
    else:
        atributes[13] = "Shut-in"
        updateStr(-4)
        updateDex(-4)
        updatePer(2)
    
    #Are they fun to be around aka Charismatic
    rando = RandomNum()
    if archetype in("Jock Sudent","Government Person"):
        rando += 10
        
    if(rando >= top):
        atributes[14] = "Charismatic"
        updateChr(10)
    
    elif(rando < top and rando > midBot):
        atributes[14] = "Average"
    
    else:
        atributes[14] = "Dull"
        updateChr(-10)
    
    #How Friendly are they
    rando = RandomNum()
    if(rando >= top):
        atributes[15] = "Friendly"
        updateChr(4)
        updatePer(-1)
    
    elif(rando < top and rando > midBot):
        atributes[15] = "Average"
    
    else:
        atributes[15] = "Unfriendly"
        updateChr(-8)
    
    return atributes


def Traits():
    global physicalTraits
    global personalityTraits
    traits = [0] * 10
    for x in traits:#to give me empty spots
        traits[x] = "0"
        
    rando = RandomNum()
    if(rando > 50):
        traits[0] = "Male"
        updateStr(10)
        updateDex(-5)
    
    else:
        traits[0] = "Female"
        updateStr(-5)
        updateDex(10)

    #Body Type
    rando = RandomNum()
    if(rando > top):
        traits[1] = "Buff"
        updateStr(7)
        updateDex(4)
        updatePrc(-2)
    
    elif(rando < top and rando > midBot):
        traits[1] = "Average"
    else:
        traits[1] = "Scrawny"
        updateStr(-7)
        updateDex(-2)
        updatePrc(5)

    #Height
    rando = RandomNum()
    if(rando > top):
        traits[2] = "Tall"
        updateDex(-3)
    
    elif(rando < top and rando > midBot):
        traits[2] = "Average"
    else:
        traits[2] = "Short"
        updateDex(4)
        
    #Hair Color
    List = ["Blonde","Brown","Black","Grey","White","Red"]
    rando = RandomNum()
    if(rando > 84):
        traits[3] = List[0]
        if(traits[0] == "Female"):
            updateInt(-20)
        
    elif(rando <= 84 and rando > 67):
        traits[3] = List[1]
        
    elif(rando <= 67 and rando > 51):
        traits[3] = List[2]
        
    elif(rando <= 51 and rando > 34):
        traits[3] = List[3]
        
    elif(rando <= 34 and rando > 17):
        traits[3] = List[4]
        
    else:
        traits[3] = List[5]
        
    #Hair Type
    List = ["Long-Curly","Short-Straight","Short-Curly","Long-Straight","Bald","Thinning","Receding"]
    rando = RandomNum()
    if(rando > 84):
        traits[4] = List[0]
        
    elif(rando <= 84 and rando > 67):
        traits[4] = List[1]
        
    elif(rando <= 67 and rando > 51):
        traits[4] = List[2]
        
    elif(rando <= 51 and rando > 34):
        traits[4] = List[3]
        
    elif(rando <= 32 and rando > 15):
        traits[4] = List[4]
        if(traits[0] == "Female"):
            traits[4] = List[5]
        
    else:
        traits[4] = List[6]

    #Skin Tone
    rando = RandomNum()
    if(rando >= 30):
        traits[5] = "White"
        
    elif(rando < 30 and rando >= 20):
        traits[5] = "Light-Brown"

    elif(rando < 20 and rando >= 10):
        traits[5] = "Brown"
    
    else:
        traits[5] = "Dark-Brown"
        
    return traits

def Run():
    global intuition
    global intelligence
    global strength
    global charisma
    global precision
    global dexterity
    global perception
    global archetype
    global canUseMagic
    global favWeapon
    global physicalTraits
    global personalityTraits
    NpcType()#Creating the Character base stats and Choosing What type they will be peasant Average Joe...
    print(archetype, end = "\n")
    if(canUseMagic):
        print("Can use Magic")

    else:
        print("Cannot use Magic")

    print("Favorite weapon type is", favWeapon, end = "\n")
    physicalTraits = Traits()#Physical Traits being made, possibaly spirt chooser
    personalityTraits = Atributes()#Personality Traits being made
    FinalizeValues()#Fixes any stat issues
    print("Intuition:", intuition, end = "\n")
    print("Intelligence:", intelligence, end = "\n")
    print("Strength:", strength, end = "\n")
    print("Charisma:", charisma, end = "\n")
    print("Precision", precision, end = "\n")
    print("Dextarity:", dexterity, end = "\n")
    print("Perception:", perception, end = "\n")
    count = 0
    for x in personalityTraits:
        if(x != 0 and x != "Average"):
            print(x, "" , end = "")
    print(end = "\n")
    #print(personalityTraits)
    for x in physicalTraits:
        count += 1
        if(x != 0 and x != "Average"):
            if(count == 6):
                print("Hair,", end = " ")
                
            print(x, "" , end = "")

    print("Skin Tone", end = "")
    
    ExportToXML(archetype, canUseMagic, favWeapon, physicalTraits, personalityTraits)
    #print(physicalTraits)


Run()
