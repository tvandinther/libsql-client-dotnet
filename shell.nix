{ pkgs ? import <nixpkgs> {} }:

pkgs.mkShell {
  buildInputs = with pkgs; [
    rustup
    cmake
  ];

  shellHook = ''
    rustup default stable
  '';
}
