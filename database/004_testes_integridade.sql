-- Execute após a carga da etapa 4 em uma base de desenvolvimento.
-- No Workbench, abra uma conexão sem transação pendente e execute em ordem
-- na mesma sessão. Se um erro interromper a execução, prossiga pela
-- instrução seguinte, até o ROLLBACK.
-- Resultados abaixo observados no MySQL 8.4.13 com a carga da etapa 4.
USE retrogames;

-- Verificação da consulta 4: a plataforma temporária apareceu com 0 jogos.
START TRANSACTION;

INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES ('Plataforma temporária da etapa 5', 'Teste', 1990);

SELECT p.co_seq_plataforma, p.no_plataforma, COUNT(j.co_seq_jogo) AS qt_jogos
FROM tb_plataforma AS p
LEFT JOIN tb_jogo AS j ON j.co_plataforma = p.co_seq_plataforma
WHERE p.co_seq_plataforma = LAST_INSERT_ID()
GROUP BY p.co_seq_plataforma, p.no_plataforma;

ROLLBACK;

-- Os IDs 2 (Super Nintendo e Usado) vêm da carga. A plataforma -1 não existe nela.
START TRANSACTION;

-- 1. Plataforma inexistente. Resultado: erro 1452 (chave estrangeira).
INSERT INTO tb_jogo (
    no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, dt_cadastro, co_plataforma, co_condicao_jogo
) VALUES ('Teste de plataforma inexistente', 1992, 10.00, 1, UTC_TIMESTAMP(), -1, 2);

-- 2. Nome de plataforma duplicado. Resultado: erro 1062 (UNIQUE).
INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES ('Super Nintendo', 'Nintendo', 1990);

-- 3. Exclusão de plataforma com jogos. Resultado: erro 1451 (chave estrangeira).
DELETE FROM tb_plataforma WHERE co_seq_plataforma = 2;

-- 4. Nome do jogo nulo. Resultado: erro 1048 (NOT NULL).
INSERT INTO tb_jogo (
    no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, dt_cadastro, co_plataforma, co_condicao_jogo
) VALUES (NULL, 1992, 10.00, 1, UTC_TIMESTAMP(), 2, 2);

-- 5. Preço zero. Resultado: aceito pelo banco. A regra RN06 pertence ao Service.
INSERT INTO tb_jogo (
    no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, dt_cadastro, co_plataforma, co_condicao_jogo
) VALUES ('Jogo temporário com preço zero', 1992, 0.00, 1, UTC_TIMESTAMP(), 2, 2);

SELECT no_jogo, vl_preco
FROM tb_jogo
WHERE co_seq_jogo = LAST_INSERT_ID();

ROLLBACK;

-- Depois do ROLLBACK, ambas as contagens retornaram zero.
SELECT COUNT(*) AS qt_plataformas_teste
FROM tb_plataforma
WHERE no_plataforma = 'Plataforma temporária da etapa 5';

SELECT COUNT(*) AS qt_jogos_teste
FROM tb_jogo
WHERE no_jogo IN ('Teste de plataforma inexistente', 'Jogo temporário com preço zero');
