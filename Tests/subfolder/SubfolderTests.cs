// ============================================================
// Project            :  Reactor
// File               :  .\game\gameScripts\Tests\subfolder\subfolderTests.cs
// Copyright          :  
// Author             :  BlueRaja
// Created on         :  Saturday, July 19, 2008 4:02 AM
//
// Editor             :  Codeweaver v. 1.2.2594.2497
//
// Description        :  
//                    :  
//                    :  
// ============================================================

TUnit::registerClass(SubfolderTests);

function SubfolderTests::subTest()
{
   Assert::isFalse(1, "This is an expected failure, displaying that tests in subdirectories get called.");
}