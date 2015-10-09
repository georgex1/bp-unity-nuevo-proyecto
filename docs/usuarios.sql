
--
-- Estructura de tabla para la tabla `usuarios`
--

CREATE TABLE IF NOT EXISTS `usuarios` (
  `id` int(11) NOT NULL auto_increment,
  `nombre` varchar(95) default NULL,
  `email` varchar(95) default NULL,
  `password` varchar(95) default NULL,
  `fecha_nacimiento` date default NULL,
  `sexo` varchar(10) default NULL,
  `fbid` varchar(255) default NULL,
  `regid` varchar(255) default NULL,
  `plataforma` varchar(45) default NULL,
  `fecha_entrada` datetime default NULL,
  `ciudad` varchar(95) default NULL,
  `foto` varchar(95) default NULL,
  PRIMARY KEY  (`id`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=1 ;


CREATE TABLE IF NOT EXISTS `notificaciones` (
  `id` int(11) NOT NULL auto_increment,
  `titulo` varchar(95) collate utf8_unicode_ci default NULL,
  `descripcion` text collate utf8_unicode_ci,
  `fecha_entrada` datetime default NULL,
  `serverupdate` timestamp NULL default CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP,
  `enviado` tinyint(1) default '0',
  `visto` tinyint(1) default '0',
  `tipo` varchar(45) collate utf8_unicode_ci default 'contenido',
  `fecha_envio` datetime default NULL,
  PRIMARY KEY  (`id`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci AUTO_INCREMENT=1 ;

CREATE TABLE IF NOT EXISTS `notificaciones_usuarios` (
  `notificaciones_id` int(11) NOT NULL,
  `usuarios_id` int(11) NOT NULL,
  `enviado` timestamp NULL default CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP,
  PRIMARY KEY  (`notificaciones_id`,`usuarios_id`),
  KEY `fk_notificaciones_usuarios_notificaciones1_idx` (`notificaciones_id`),
  KEY `fk_notificaciones_usuarios_usuarios1_idx` (`usuarios_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

