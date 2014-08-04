﻿' Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports System
Imports System.Collections.Immutable
Imports System.Runtime.InteropServices
Imports System.Threading
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports TypeKind = Microsoft.CodeAnalysis.TypeKind

Namespace Microsoft.CodeAnalysis.VisualBasic

    Friend Class SynthesizedLambdaConstructor
        Inherits SynthesizedMethod
        Implements ISynthesizedMethodBodyImplementationSymbol

        Friend Sub New(
            syntaxNode As VisualBasicSyntaxNode,
            containingType As LambdaFrame
        )
            MyBase.New(syntaxNode, containingType, WellKnownMemberNames.InstanceConstructorName, False)
        End Sub

        Public Overrides ReadOnly Property MethodKind As MethodKind
            Get
                Return MethodKind.Constructor
            End Get
        End Property

        Friend Function AsMember(frameType As NamedTypeSymbol) As MethodSymbol
            ' ContainingType is always a Frame here which is a type definition so we can use "Is"
            If frameType Is ContainingType Then
                Return Me
            End If

            Dim substituted = DirectCast(frameType, SubstitutedNamedType)
            Return DirectCast(substituted.GetMemberForDefinition(Me), MethodSymbol)
        End Function

        Friend NotOverridable Overrides ReadOnly Property HasSpecialName As Boolean
            Get
                Return True
            End Get
        End Property

        Friend Overrides Sub AddSynthesizedAttributes(ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
            MyBase.AddSynthesizedAttributes(attributes)

            ' Dev11 adds DebuggerNonUserCode; there is no reason to do so since:
            ' - we emit no debug info for the body
            ' - the code doesn't call any user code that could inspect the stack and find the accessor's frame
            ' - the code doesn't throw exceptions whose stack frames we would need to hide
            ' 
            ' C# also doesn't add DebuggerHidden nor DebuggerNonUserCode attributes.
        End Sub

        Friend Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
            Get
                Return False
            End Get
        End Property

        Friend Overrides Function IsMetadataNewSlot(Optional ignoreInterfaceImplementationChanges As Boolean = False) As Boolean
            Return False
        End Function

        Public ReadOnly Property HasMethodBodyDependency As Boolean Implements ISynthesizedMethodBodyImplementationSymbol.HasMethodBodyDependency
            Get
                Return False
            End Get
        End Property

        Public ReadOnly Property Method As IMethodSymbol Implements ISynthesizedMethodBodyImplementationSymbol.Method
            Get
                Dim symbol As ISynthesizedMethodBodyImplementationSymbol = CType(ContainingSymbol, ISynthesizedMethodBodyImplementationSymbol)
                Return symbol.Method
            End Get
        End Property
    End Class

End Namespace