clear all;


%% User area
outputFilename = ".\0_800_2VS.swp";
lines = 800/2; % Only for up-cycle. Downcycle will be generated automatically
% lines = lines * 5; % Outline for 10V steps
lines = lines+1;
repeats = [1 300]; % normal repeats; 300 repeats at max
controlParameter = ["Rpts" "U1" ...
                        %   "U2" ...
                        %   "H3" "L3" "F3" "D3" ...
                   ];
sweepParameters = [[0 800]; ...
                  %  [1000 1000]; ...
                  % [5 5]; [0 0]; [1000 1000]; [1 99]; ...
                  ]; % linewise! separate your control-variables with semikolon -> [[1]; [2]];
% controlParameter = ["Rpts" "U1"];
% sweepParameters = [[0 500]];




%% Do not touch area!11elf
pars(:, 1) = [linspace(repeats(1), repeats(1), lines-1) repeats(2) linspace(repeats(1), repeats(1), lines-1)];
for i = 2:size(controlParameter, 2)
    schrittweite = (sweepParameters(i-1, 2) - sweepParameters(i-1, 1)) / (lines - 1);
    pars(:, i) = [ [sweepParameters(i-1, 1):schrittweite:sweepParameters(i-1, 2)-schrittweite] [sweepParameters(i-1, 2)] [sweepParameters(i-1, 2)-schrittweite:-schrittweite:sweepParameters(i-1, 1)] ]';
%     tmp = [ [sweepParameters(i-1, 1):schrittweite:sweepParameters(i-1, 2)-schrittweite] [sweepParameters(i-1, 2)] [sweepParameters(i-1, 2)-schrittweite:-schrittweite:sweepParameters(i-1, 1)] ]';
end

str = string(pars);
str(2:end+1, :) = str;
str(1, :) = controlParameter;
for i = 1:size(str, 2)-1
   str(:, i) = str(:, i) + ","; 
end

if isfile(outputFilename)
    delete(outputFilename);
end

fid = fopen(outputFilename, 'a');
for line = 1:size(str, 1)
    buffer = str(line, :);
    fprintf(fid, "%-10s", buffer);
    fprintf(fid, '\r\n');
end
fclose(fid);


