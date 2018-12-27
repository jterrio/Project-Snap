import math
import random
#Ryan = "is a buthead"
intuition = 0
intelligence = 0
strength = 0
charisma = 0
precision = 0
dexterity = 0
perception = 0
def Character(startingStat,startingPer):
    global intuition
    global intelligence
    global strength
    global charisma
    global precision
    global dexterity
    global perception
    intuition = startingStat #und - ability to pickup skills in combat
    intelligence = startingStat #int - ability to study skills outside of combat
    strength = startingStat #str - ability to swing hard for physical attacks
    charisma = startingStat #chr - ability to get people to like you
    precision = startingStat #prc - ability to aim
    spirituality = 0 #spr - ability to survive more attacks
    dexterity = startingStat #dex - ability to use more attacks or move
    perception = startingPer #per - ability to process info in combat (default time per turn is 0.75seconds. every level reduces this by 0.025, with a hard cap at 20 points)
        
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
                
            
        
    

top = 70
midBot = 31
#returns a number bewtween 1 and 100
def RandomNum():
    return math.floor(random.randint(1,100))

#returns what type of npc they are going to be and the starting stats
def NpcType():
    rando = RandomNum()
    #Goverment People
    if(rando >= 80):
        Character(40,9)
        print("Goverment Person", end = '\n')

    #Average Person
    elif(rando < 80 and rando > 50):
        Character(20,7)
        print("Average Person", end = '\n')

    #Peasant
    elif(rando <= 59 and rando > 10):
        Character(10,4)
        print("Peasant", end = '\n')
    #Hero Type
    else:
        Character(60,13)
        print("Hero", end = '\n')

#creates an array of atributes to define an npc
def Atributes():
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
    if(rando >= top):
        atributes[6] = "Obedient"
        updatePrc(2)
    
    elif(rando < top and rando > midBot):
        atributes[6] = "Average"
    
    else:
        atributes[6] = "Disobedient"
        updateDex(2)
    
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

    #Hair Type
    rando = RandomNum()
    if(rando > 84):
        traits[3] = "Long-Curly"
        
    elif(rando <= 84 and rando > 67):
        traits[3] = "Short-Straight"
        
    elif(rando <= 67 and rando > 51):
        traits[3] = "Short-Curly"
        
    elif(rando <= 51 and rando > 34):
        traits[3] = "Long-Straight"
        
    elif(rando <= 34 and rando > 17):
        traits[3] = "Bald"
        
    else:
        traits[3] = "Receding"

    #Hair Color
    rando = RandomNum()
    if(rando > 84):
        traits[4] = "Blonde"
        if(traits[0] == "Female"):
            updateInt(-20)
        
    elif(rando <= 84 and rando > 67):
        traits[4] = "Brown"
        
    elif(rando <= 67 and rando > 51):
        traits[4] = "Black"
        
    elif(rando <= 51 and rando > 34):
        traits[4] = "Grey"
        
    elif(rando <= 34 and rando > 17):
        traits[4] = "White"
        
    else:
        traits[4] = "Red"
        
    return traits

def Run():
    global intuition
    global intelligence
    global strength
    global charisma
    global precision
    global dexterity
    global perception
    NpcType()#Creating the Character base stats and Choosing What type they will be peasant Average Joe...
    personalityTraits = Atributes()#Personality Traits being made
    physicalTraits = Traits()#Physical Traits being made, possibaly spirt chooser
    #FinalizeValues()#Fixes any stat issues
    print("Intuition:", intuition, end = "\n")
    print("Intelligence:", intelligence, end = "\n")
    print("Strength:", strength, end = "\n")
    print("Charisma:", charisma, end = "\n")
    print("Precision", precision, end = "\n")
    print("Dextarity:", dexterity, end = "\n")
    print("Perception:", perception, end = "\n")
    for x in personalityTraits:
        if(x != 0 and x != "Average"):
            print(x, "" , end = '')
    print(end = "\n")
    #print(personalityTraits)
    for x in physicalTraits:
        if(x != 0 and x != "Average"):
            print(x, "" , end = '')
    #print(physicalTraits)


Run()
