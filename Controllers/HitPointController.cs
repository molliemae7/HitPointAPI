
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HitPointAPI.Controllers
{
    [Route("api/HitPoint")]
    [ApiController]
    public class HitPointController : ControllerBase
    {
        private readonly ILogger<HitPointController> _logger;

        public HitPointController(ILogger<HitPointController> logger)
        {
            _logger = logger;
        }

        // GET api/hitPoint
        [HttpGet]
        public ActionResult<int> Get(string characterName)
        {
            var character = LoadJson(characterName); 
            int hp = InitializeHitPoints(character);
                var tempHPValues = new HPValues();
                tempHPValues.currentHP=hp;
                tempHPValues.tempHP=0;
                tempHPValues.maxHP=hp;
                string sessionKey="";
                sessionKey = character.name;
                var value = HttpContext.Session.GetString(sessionKey);
                if (string.IsNullOrEmpty(value))
                {
                    var serialisedHPValues = JsonConvert.SerializeObject(tempHPValues);
                    HttpContext.Session.SetString(sessionKey, serialisedHPValues);
                }    
            
            return tempHPValues.currentHP;
        }

        // POST api/hitPoint
        [HttpPost]
        public ActionResult<HPValues> PostHitPoint([FromBody]HPChange myChange, string characterName)
        {
            string value = HttpContext.Session.GetString(characterName);
            var tempHPValues = JsonConvert.DeserializeObject<HPValues>(value);
            var character = LoadJson(characterName);
            switch(myChange.changeType){
                case "heal":
                    if(myChange.changeMode=="static"){
                        tempHPValues.currentHP+=myChange.changeValue;
                    } else if(myChange.changeMode=="dice"){
                        var rand = new Random();
                        int randDiceRoll = rand.Next(1, myChange.changeValue+1);
                        tempHPValues.currentHP+=randDiceRoll+myChange.changeModifier;
                    }
                    if(tempHPValues.currentHP>tempHPValues.maxHP)
                        tempHPValues.currentHP=tempHPValues.maxHP;
                    break;
                case "damage":
                    if(myChange.changeMode=="static"){
                        int damageTotal = myChange.changeValue;
                        for(int i=0;i<character.defenses.Count;i++){
                            if(character.defenses[i].type==myChange.damageType){
                                if(character.defenses[i].defense=="immunity")
                                    damageTotal=0;
                                if(character.defenses[i].defense=="resistance")
                                    damageTotal=Convert.ToInt32(Math.Floor(Convert.ToDecimal(damageTotal)/2));
                            }
                        }
                            int tempHPValue=tempHPValues.tempHP;
                            if(tempHPValues.tempHP>damageTotal)
                                tempHPValues.tempHP-=damageTotal;
                            else if(tempHPValues.tempHP==damageTotal)
                                tempHPValues.tempHP=0;
                            else{
                                tempHPValues.tempHP=0;
                                tempHPValues.currentHP-=damageTotal+tempHPValue;
                            }
                    } else if(myChange.changeMode=="dice"){
                        var rand = new Random();
                        int randDiceRoll = rand.Next(1, myChange.changeValue+1);
                        int damageTotal = randDiceRoll+myChange.changeModifier;
                        for(int i=0;i<character.defenses.Count;i++){
                            if(character.defenses[i].type==myChange.damageType){
                                if(character.defenses[i].defense=="immunity")
                                    damageTotal=0;
                                if(character.defenses[i].defense=="resistance")
                                    damageTotal=Convert.ToInt32(Math.Floor(Convert.ToDecimal(damageTotal)/2));
                            }
                        }
                        if(tempHPValues.tempHP>damageTotal)
                            tempHPValues.tempHP-=damageTotal;
                        else if(tempHPValues.tempHP==damageTotal)
                            tempHPValues.tempHP=0;
                        else{
                            tempHPValues.currentHP-=damageTotal;
                            tempHPValues.currentHP+=tempHPValues.tempHP;
                            tempHPValues.tempHP=0;
                        }
                    }
                    if(tempHPValues.currentHP<0)
                        tempHPValues.currentHP=0;
                    break;
                case "tempHP": 
                    if(myChange.changeMode=="static"){
                        tempHPValues.tempHP+=myChange.changeValue;
                     } else if(myChange.changeMode=="dice"){
                        var rand = new Random();
                        int randDiceRoll = rand.Next(1, myChange.changeValue+1);
                        tempHPValues.tempHP+=randDiceRoll+myChange.changeModifier;
                     }
                    break;
            }
            var serialisedHPValues = JsonConvert.SerializeObject(tempHPValues);
            HttpContext.Session.SetString(characterName, serialisedHPValues);
            return tempHPValues;
        }


        public int InitializeHitPoints(Character character)
        {
            int hp = 0;
            for(int i =0; i<character.classes.Count;i++){
                int hdValue = character.classes[i].hitDiceValue;
                int clsLvl = character.classes[i].classLevel;
                decimal conCalc = (character.stats.constitution-10)/2;
                int conBonus = Convert.ToInt32(Math.Floor(conCalc));
                hp+=(hdValue/2+1+conBonus)*clsLvl;
            }
            return hp;
        }

        public Character LoadJson(string characterName)
        {
            using (StreamReader r = new StreamReader(characterName.ToLower() + ".json"))
            {
                string json = r.ReadToEnd();
                var character = JsonConvert.DeserializeObject<Character>(json);
                var affectedCharacter = itemEffect(character);
               
                return character;
            }
        }
        public Character itemEffect(Character character){
            Character affectedCharacter = character;
            for(int i=0;i<character.items.Count;i++){
                if(character.items[i].modifier.affectedObject=="stats"){
                    switch(character.items[i].modifier.affectedValue){
                        case "strength":
                            affectedCharacter.stats.strength+= character.items[i].modifier.value;
                            break;
                        case "dexterity":
                            affectedCharacter.stats.dexterity+= character.items[i].modifier.value;
                            break;
                        case "constitution":
                            affectedCharacter.stats.constitution+= character.items[i].modifier.value;
                            break;
                        case "intelligence":
                            affectedCharacter.stats.intelligence+= character.items[i].modifier.value;
                            break;
                        case "wisdom":
                            affectedCharacter.stats.wisdom+= character.items[i].modifier.value;
                            break;
                        case "charisma":
                            affectedCharacter.stats.charisma+= character.items[i].modifier.value;
                            break;
                    }
                }
            }
            return affectedCharacter;
        }
    }
}


