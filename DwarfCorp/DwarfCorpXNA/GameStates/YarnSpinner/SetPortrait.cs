﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DwarfCorp.GameStates.YarnSpinner
{

    static class EmployeeCommands
    {
        [YarnCommand("pay_employee_bonus", ArgumentTypeBehavior = YarnCommandAttribute.ArgumentTypeBehaviors.Strict)]
        private static void _pay_employee(YarnEngine State, List<Ancora.AstNode> Arguments, Yarn.MemoryVariableStore Memory)
        {
            var employee = Memory.GetValue("$employee");
            var creature = employee.AsObject as CreatureAI;
            if (creature == null)
                return;
            var bonus = Memory.GetValue("$employee_bonus").AsNumber;
            creature.AddMoney((decimal)(float)bonus);
            creature.Faction.AddMoney(-(decimal)(float)bonus);
        }

        [YarnCommand("add_thought", "STRING", "NUMBER", "NUMBER", ArgumentTypeBehavior = YarnCommandAttribute.ArgumentTypeBehaviors.Strict)]
        private static void _add_thought(YarnEngine State, List<Ancora.AstNode> Arguments, Yarn.MemoryVariableStore Memory)
        {
            var employee = Memory.GetValue("$employee");
            var creature = employee.AsObject as CreatureAI;
            if (creature == null)
                return;
            creature.Creature.AddThought(new Thought()
            {
                Description = (string)Arguments[0].Value,
                TimeLimit = new TimeSpan((int)(float)Arguments[2].Value, 0, 0),
                HappinessModifier = (float)Arguments[1].Value,
                Type = Thought.ThoughtType.Other,
                TimeStamp = creature.World.Time.CurrentDate
            }, false);
        }

        [YarnCommand("end_strike", ArgumentTypeBehavior = YarnCommandAttribute.ArgumentTypeBehaviors.Strict)]
        private static void _end_strike(YarnEngine State, List<Ancora.AstNode> Arguments, Yarn.MemoryVariableStore Memory)
        {
            var employee = Memory.GetValue("$employee");
            var creature = employee.AsObject as CreatureAI;
            if (creature == null)
                return;
            creature.Status.IsOnStrike = false;
        }
    }

    static class SetPortrait
    {
        [YarnCommand("set_portrait", "STRING", "NUMBER", "NUMBER", "NUMBER", "NUMBER", ArgumentTypeBehavior = YarnCommandAttribute.ArgumentTypeBehaviors.LastIsVaridic)]
        private static void _set_portrait(YarnEngine State, List<Ancora.AstNode> Arguments, Yarn.MemoryVariableStore Memory)
        {
            var frames = new List<int>();
            for (var i = 4; i < Arguments.Count; ++i)
                frames.Add((int)((float)Arguments[i].Value));
            State.PlayerInterface.SetPortrait((string)Arguments[0].Value, (int)((float)Arguments[1].Value), (int)((float)Arguments[2].Value), (float)Arguments[3].Value, frames);
        }
    }
}