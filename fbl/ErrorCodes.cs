namespace FBL
{
    public enum ErrorCodes
    {
        // Start up
        C_FileDoesNotExists = 1,


        // Tokenization
        T_UnexpectedEndOfFile = 10,

        T_InvalidIdentifier = 20,

        T_InvalidCompilerDirectiveName = 21,
        T_CompilerDirectiveNameIsNotStated = 22,

        T_SpecialCharacterDoesNotExists = 30,
        T_MisleadingCharacter = 31,


        // Parsing
        P_IdentifierExpected = 100,
        P_MethodTypeExpected = 101,
        P_MethodNameExpected = 102,
        P_OpeningBracketExpected = 103,

        P_DuplicatedModifier = 110,
        P_ReferenceCanNotBeConstant = 111,
        P_ArrayCanNotContainReferences = 112,

        P_ColonBeforeTypeSpeceficationNotFound = 120,

        P_UnknownUnit = 130,
        P_ClosingBraceRequired = 131,



        // Optimization
        // ...


        // Translation
        // ...


        // Compilation
        // ...
    }
}
