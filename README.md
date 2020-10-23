# HitPointAPI
Assume the character JSON file has already been uploaded.

Call https://localhost:5001/api/hitPoint/:
GET Function to load character and intialize hit point store:
Params:
	CharacterName	string

POST function to take action
Params:
	CharacterName 	string
Body:
	Pass JSON:
	{
	    "changeType": ["damage",'heal","tempHP"],
 	    "changeMode": ["static","dice"],
	    "changeValue": 8, //indicates either the static vaule or the die value for "Dice" variant
	    "changeModifier": 3, //only applicable for "dice" variation
	    "damageType": ["fire","cold","bludgeoning","slashing",etc] //only applicable for "damage" variation
	}
