// Generated by TinyPG v1.3 available at www.codeproject.com

using System;
using System.Collections.Generic;

namespace TinyPG
{
    #region Parser

    public partial class Parser 
    {
        private Scanner scanner;
        private ParseTree tree;
        
        public Parser(Scanner scanner)
        {
            this.scanner = scanner;
        }

        public ParseTree Parse(string input)
        {
            tree = new ParseTree();
            return Parse(input, tree);
        }

        public ParseTree Parse(string input, ParseTree tree)
        {
            scanner.Init(input);

            this.tree = tree;
            ParseStart(tree);
            tree.Skipped = scanner.Skipped;

            return tree;
        }

        private void ParseStart(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Start), "Start");
            parent.Nodes.Add(node);


            
            tok = scanner.LookAhead(TokenType.CLASS);
            while (tok.Type == TokenType.CLASS)
            {
                ParseClass(node);
            tok = scanner.LookAhead(TokenType.CLASS);
            }

            
            tok = scanner.Scan(TokenType.EOF);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.EOF) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.EOF.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                return;
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParsefControls(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.fControls), "fControls");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.FCONTROLS);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.FCONTROLS) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.FCONTROLS.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                return;
            }

            
            tok = scanner.Scan(TokenType.CODEBLOCKOPEN);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.CODEBLOCKOPEN) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.CODEBLOCKOPEN.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                return;
            }

            
            tok = scanner.LookAhead(TokenType.PROPERTY, TokenType.FCONTROLS, TokenType.BCONTROLS, TokenType.CLASS);
            while (tok.Type == TokenType.PROPERTY
                || tok.Type == TokenType.FCONTROLS
                || tok.Type == TokenType.BCONTROLS
                || tok.Type == TokenType.CLASS)
            {
                tok = scanner.LookAhead(TokenType.PROPERTY, TokenType.FCONTROLS, TokenType.BCONTROLS, TokenType.CLASS);
                switch (tok.Type)
                {
                    case TokenType.PROPERTY:
                        tok = scanner.Scan(TokenType.PROPERTY);
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        if (tok.Type != TokenType.PROPERTY) {
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.PROPERTY.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                            return;
                        }
                        break;
                    case TokenType.FCONTROLS:
                        ParsefControls(node);
                        break;
                    case TokenType.BCONTROLS:
                        ParsebControls(node);
                        break;
                    case TokenType.CLASS:
                        ParseControl(node);
                        break;
                    default:
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, 0, tok.StartPos, tok.StartPos, tok.Length));
                        break;
                }
            tok = scanner.LookAhead(TokenType.PROPERTY, TokenType.FCONTROLS, TokenType.BCONTROLS, TokenType.CLASS);
            }

            
            tok = scanner.Scan(TokenType.CODEBLOCKCLOSE);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.CODEBLOCKCLOSE) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.CODEBLOCKCLOSE.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                return;
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParsebControls(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.bControls), "bControls");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.BCONTROLS);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.BCONTROLS) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.BCONTROLS.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                return;
            }

            
            tok = scanner.Scan(TokenType.CODEBLOCKOPEN);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.CODEBLOCKOPEN) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.CODEBLOCKOPEN.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                return;
            }

            
            tok = scanner.LookAhead(TokenType.PROPERTY, TokenType.FCONTROLS, TokenType.BCONTROLS, TokenType.CLASS);
            while (tok.Type == TokenType.PROPERTY
                || tok.Type == TokenType.FCONTROLS
                || tok.Type == TokenType.BCONTROLS
                || tok.Type == TokenType.CLASS)
            {
                tok = scanner.LookAhead(TokenType.PROPERTY, TokenType.FCONTROLS, TokenType.BCONTROLS, TokenType.CLASS);
                switch (tok.Type)
                {
                    case TokenType.PROPERTY:
                        tok = scanner.Scan(TokenType.PROPERTY);
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        if (tok.Type != TokenType.PROPERTY) {
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.PROPERTY.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                            return;
                        }
                        break;
                    case TokenType.FCONTROLS:
                        ParsefControls(node);
                        break;
                    case TokenType.BCONTROLS:
                        ParsebControls(node);
                        break;
                    case TokenType.CLASS:
                        ParseControl(node);
                        break;
                    default:
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, 0, tok.StartPos, tok.StartPos, tok.Length));
                        break;
                }
            tok = scanner.LookAhead(TokenType.PROPERTY, TokenType.FCONTROLS, TokenType.BCONTROLS, TokenType.CLASS);
            }

            
            tok = scanner.Scan(TokenType.CODEBLOCKCLOSE);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.CODEBLOCKCLOSE) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.CODEBLOCKCLOSE.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                return;
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseControl(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Control), "Control");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.CLASS);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.CLASS) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.CLASS.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                return;
            }

            
            tok = scanner.Scan(TokenType.CODEBLOCKOPEN);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.CODEBLOCKOPEN) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.CODEBLOCKOPEN.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                return;
            }

            
            tok = scanner.LookAhead(TokenType.PROPERTY, TokenType.FCONTROLS, TokenType.BCONTROLS, TokenType.CLASS);
            while (tok.Type == TokenType.PROPERTY
                || tok.Type == TokenType.FCONTROLS
                || tok.Type == TokenType.BCONTROLS
                || tok.Type == TokenType.CLASS)
            {
                tok = scanner.LookAhead(TokenType.PROPERTY, TokenType.FCONTROLS, TokenType.BCONTROLS, TokenType.CLASS);
                switch (tok.Type)
                {
                    case TokenType.PROPERTY:
                        tok = scanner.Scan(TokenType.PROPERTY);
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        if (tok.Type != TokenType.PROPERTY) {
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.PROPERTY.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                            return;
                        }
                        break;
                    case TokenType.FCONTROLS:
                        ParsefControls(node);
                        break;
                    case TokenType.BCONTROLS:
                        ParsebControls(node);
                        break;
                    case TokenType.CLASS:
                        ParseControl(node);
                        break;
                    default:
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, 0, tok.StartPos, tok.StartPos, tok.Length));
                        break;
                }
            tok = scanner.LookAhead(TokenType.PROPERTY, TokenType.FCONTROLS, TokenType.BCONTROLS, TokenType.CLASS);
            }

            
            tok = scanner.Scan(TokenType.CODEBLOCKCLOSE);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.CODEBLOCKCLOSE) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.CODEBLOCKCLOSE.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                return;
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseClass(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Class), "Class");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.CLASS);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.CLASS) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.CLASS.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                return;
            }

            
            tok = scanner.Scan(TokenType.CODEBLOCKOPEN);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.CODEBLOCKOPEN) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.CODEBLOCKOPEN.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                return;
            }

            
            tok = scanner.LookAhead(TokenType.PROPERTY, TokenType.FCONTROLS, TokenType.BCONTROLS, TokenType.CLASS);
            while (tok.Type == TokenType.PROPERTY
                || tok.Type == TokenType.FCONTROLS
                || tok.Type == TokenType.BCONTROLS
                || tok.Type == TokenType.CLASS)
            {
                tok = scanner.LookAhead(TokenType.PROPERTY, TokenType.FCONTROLS, TokenType.BCONTROLS, TokenType.CLASS);
                switch (tok.Type)
                {
                    case TokenType.PROPERTY:
                        tok = scanner.Scan(TokenType.PROPERTY);
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        if (tok.Type != TokenType.PROPERTY) {
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.PROPERTY.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                            return;
                        }
                        break;
                    case TokenType.FCONTROLS:
                        ParsefControls(node);
                        break;
                    case TokenType.BCONTROLS:
                        ParsebControls(node);
                        break;
                    case TokenType.CLASS:
                        ParseControl(node);
                        break;
                    default:
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, 0, tok.StartPos, tok.StartPos, tok.Length));
                        break;
                }
            tok = scanner.LookAhead(TokenType.PROPERTY, TokenType.FCONTROLS, TokenType.BCONTROLS, TokenType.CLASS);
            }

            
            tok = scanner.Scan(TokenType.CODEBLOCKCLOSE);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.CODEBLOCKCLOSE) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.CODEBLOCKCLOSE.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                return;
            }

            parent.Token.UpdateRange(node.Token);
        }


    }

    #endregion Parser
}