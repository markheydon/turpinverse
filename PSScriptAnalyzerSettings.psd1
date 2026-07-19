@{
  Rules = @{
    PSAvoidUsingCmdletAliases            = @{ Severity = 'Warning' }
    PSUseApprovedVerbs                   = @{ Severity = 'Error' }
    PSUseDeclaredVarsMoreThanAssignments = @{ Severity = 'Warning' }
    PSProvideCommentHelp                 = @{ Severity = 'Warning' }
  }

  ExcludeRules = @(
    'PSUseShouldProcessForStateChangingFunctions'
    'PSUseBOMForUnicodeEncodedFile'
    'PSAvoidUsingWriteHost'
  )
}
