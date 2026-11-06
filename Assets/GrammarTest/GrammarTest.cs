using System.Collections.Generic;
using UnityEngine;
using System;

public class GrammarTest: MonoBehaviour
{

    private void Start()
    {
        for (int i = 0; i < 10; i++) { PrintChordSequence(GetSectionChordSequence_Ionian()); }
    }

    // the macrostructure becomes symbols, A, B, C, D.
    // These are not true gramatical symbols, but bins, each with their own orgin symbols.
    // once an A, B is generated, subsequent bins will match
    // therefore for ABAB, only one A, and one B needs to actually be generated (for now)
    // we dont do development (in the musical sense)

    // within the A, Bs we have a small sequence of orgin symbols, genererated for now arbitrarialy
    // these are all 'c' or 'o', closed or open
    // c and o becomes a sequence of function-categorical symbols.
    // these symbols are T, Ts, D, Ds, SD
    // c always ends on a tonic, or tonic sub. 
    // o always ends on a dominant or dminant sub

    // we then for each of these categories, substitute a chord arbitrarialy for the set.

    //Technical considerations on symbols
    //  - for c, and o, booleans can be used or an enumeration (for extendability later)
    //  - for T,Ts,D,Ds,SD, a enumeration can be used
    //  - for the discrete chords, integers could be used, mapped to roman numerals, or enumerations
    //      - we will need to map these to chords later. Enums can have function extensions that may be useful

    // enumerations
    public enum PassageOpen { Open, Closed }
    public enum FunctionCategory { Tonic = 0, TonicSub = 1, Dominant = 2, DominantSub = 3, Subdominant = 4 }
    public enum Chord_CommonNotation { I, II, III, IV, V, VI, VII }
    // DICTIONARIES AND LISTS, DATA
    // sequences of orgin symbols
    private List<List<PassageOpen>> _orginSequences = new() { 
        new() {PassageOpen.Closed, PassageOpen.Closed }, //cc
        new() {PassageOpen.Closed, PassageOpen.Open }, //co
        new() {PassageOpen.Closed, PassageOpen.Open, PassageOpen.Closed }, //coc
        new() {PassageOpen.Closed, PassageOpen.Open, PassageOpen.Open, PassageOpen.Closed }, //cooc
    };
    // function mapping for Ionian with common chords
    private Dictionary<FunctionCategory, List<Chord_CommonNotation>> _ionianFunctionalBindings = new()
    {
        { FunctionCategory.Tonic, new() {Chord_CommonNotation.I } },
        { FunctionCategory.TonicSub, new() {Chord_CommonNotation.III, Chord_CommonNotation.VI, } },
        { FunctionCategory.Dominant, new() {Chord_CommonNotation.V } },
        { FunctionCategory.DominantSub, new() {Chord_CommonNotation.V, Chord_CommonNotation.VII, } },
        { FunctionCategory.Subdominant, new() {Chord_CommonNotation.II, Chord_CommonNotation.IV } },
    };



    // generates the sequence of chord functions procedurally from the passage status
    private List<FunctionCategory> GenerateFunctionSequence(PassageOpen passageOpen)
    {
        // algorithm is not specified for this. We should seek to generate four chords, though 5 is acceptable.
        // the last chord is the only constrained chord
        int CHORDQUANTITY = 4;
        List<FunctionCategory> chordFunctions = new();
        System.Random rand = new();
        for(int i = 0; i < CHORDQUANTITY - 1; i++) { chordFunctions.Add((FunctionCategory)rand.Next(0, 4)); }
        if (passageOpen == PassageOpen.Open) {
            chordFunctions.Add(rand.Next(0, 1) == 0 ? FunctionCategory.Dominant : FunctionCategory.DominantSub); }
        if (passageOpen == PassageOpen.Closed) {
            chordFunctions.Add(rand.Next(0, 1) == 0 ? FunctionCategory.Tonic : FunctionCategory.TonicSub); }

        return chordFunctions;
    }
    private List<List<Chord_CommonNotation>> GetSectionChordSequence_Ionian()
    {
        System.Random rand = new();
        // gets orgin symbols
        List<PassageOpen> passageOpenSequence = _orginSequences[rand.Next(0, _orginSequences.Count - 1)];
        // gets functional sequences
        List<List<FunctionCategory>> categorySets = new();
        for (int i = 0; i < passageOpenSequence.Count; i++) { categorySets.Add(GenerateFunctionSequence(passageOpenSequence[i])); }
        List<List<Chord_CommonNotation>> output = new();
        //converts functions to common chords
        for(int i = 0; i <  categorySets.Count; i++)
        {
            output.Add(new());
            for(int j = 0; j < categorySets[i].Count; j++)
            {
                output[^1].Add(_ionianFunctionalBindings[categorySets[i][j]][rand.Next(0, _ionianFunctionalBindings[categorySets[i][j]].Count - 1)]);
            }
        }

        return output;
    }

    // Print Test
    private void PrintChordSequence(List<List<Chord_CommonNotation>> chords)
    {
        string printout = "Sequence ---\n";
        for(int i = 0; i < chords.Count; i++)
        {
            printout += $"Passage {i + 1}: ";
            for(int j = 0; j < chords[i].Count; j++)
            {
                printout += chords[i][j].ToString() + ", ";
            }
            printout += "\n";
        }
        Debug.Log(printout);
    }

}
